using System;
using System.Net.Http;
using System.Threading;
using Polly.Wants.A.Cracker.Common.Model;
using Polly.Wants.A.Cracker.Common.Services;
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
                .Or<NullReferenceException>()
                .CircuitBreaker(5, TimeSpan.FromSeconds(10));



            _retryPolicy = Policy.Handle<HttpRequestException>()
                .Or<NullReferenceException>()
                .RetryForever(ex => Log.Information("Retrying..."));

            LoopCalls();
        }

        static void LoopCalls()
        {
            while (true)
            {
                try
                {
                    Log.Information("Calling api to get values...");

                    _retryPolicy.Wrap(_circuitBreakerPolicy)
                        .Execute(() =>
                        {
                            // substitute in IoC in real world scenario
                            IJsonService service = new JsonService(new HttpClient());

                            var users = service.GetUsersWithExceptions(DateTime.Now.Second);

                            if (users == null)
                            {
                                throw new HttpRequestException();
                            }

                            foreach (User user in users)
                            {
                                Log.Information("User Name : {Name}", user.name);
                            }

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
