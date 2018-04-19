using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Wants.A.Cracker.Common.Model;
using Polly.Wants.A.Cracker.Common.Services;
using Polly.Wrap;
using Serilog;

namespace Polly.Wants.A.Cracker.UI
{
    class Program
    {
        static Policy _circuitBreakerPolicy;
        static Policy _retryPolicy;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                .CircuitBreaker(
                   // number of exceptions before breaking circuit
                   5,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                       // on circuit opened
                       Log.Information("Circuit breaker opened");
                   },
                   () =>
                   {
                       // on circuit closed
                       Log.Information("Circuit breaker reset");
                   });



            _retryPolicy = Policy.Handle<HttpRequestException>()
                .WaitAndRetry(
                    // number of retries
                    4,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.ExecutionKey}";
                        Log.Warning(msg);
                    });

            // Define a fallback policy: provide a nice substitute message to the user, if we found the circuit was broken.
            FallbackPolicy<UserPayload> fallbackForCircuitBreaker = Policy<UserPayload>
                .Handle<BrokenCircuitException>()
                .Fallback(
                    fallbackValue: new UserPayload { Users = null, ErrorMessage = "Please try again later [Fallback for broken circuit]" },
                    onFallback: b =>
                    {
                        Log.Information("Fallback catches failed with: " + b.Exception.Message, ConsoleColor.Red);                    }
                );

            // Define a fallback policy: provide a substitute string to the user, for any exception.
            FallbackPolicy<UserPayload> fallbackForAnyException = Policy<UserPayload>
                .Handle<Exception>()
                .Fallback(
                    fallbackAction: /* Demonstrates fallback action/func syntax */ () => { return new UserPayload { Users = null, ErrorMessage = "Please try again later [Fallback for any exception]" }; },
                    onFallback: e =>
                    {
                        Log.Information("Fallback catches failed with: " + e.Exception.Message, ConsoleColor.DarkMagenta);
                    }
);

            PolicyWrap myResilienceStrategy = Policy.Wrap(_retryPolicy, _circuitBreakerPolicy);
            PolicyWrap<UserPayload> policyWrap = fallbackForAnyException.Wrap(fallbackForCircuitBreaker.Wrap(myResilienceStrategy));
            LoopCalls(policyWrap);
        }

        static void LoopCalls(PolicyWrap<UserPayload> policyWrap)
        {
            while (true)
            {
                try
                {
                    Log.Information("Calling api to get values...");

                    policyWrap
                        .Execute(() =>
                        {
                            // substitute in IoC in real world scenario
                            IJsonService service = new JsonService(new HttpClient());

                            //change the parameter here to 4 to trigger errors every time.
                            var users = service.GetUsersWithExceptions(DateTime.Now.Second);

                            if (users == null)
                            {
                                throw new HttpRequestException();
                            }

                            foreach (User user in users)
                            {
                                Log.Information("User Name : {Name}", user.name);
                            }

                            return new UserPayload { Users = users };

                        });

                    Thread.Sleep(2000);

                }
                catch (Exception ex)
                {
                    Log.Error("Error occured in application : {Name}", ex.Message);
                    Thread.Sleep(1000);
                }
            }

        }
    }
}
