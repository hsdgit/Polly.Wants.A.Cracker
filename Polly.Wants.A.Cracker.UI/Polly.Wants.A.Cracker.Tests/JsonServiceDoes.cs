using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Polly.Wants.A.Cracker.Common.Model;
using Polly.Wants.A.Cracker.Common.Services;
using Shouldly;
using Xunit;


namespace Polly.Wants.A.Cracker.Tests
{
    public class JsonServiceDoes
    {
        [Fact]
        public void Immplements_IJsonService()
        {
            typeof(IJsonService).IsAssignableFrom(typeof(JsonService));
        }

        [Fact]
        public void returns_an_enumerable_of_users()
        {
            IJsonService jsonService = Substitute.For<IJsonService>();
            jsonService.GetUsers().Returns(new List<User>());

            jsonService.GetUsers().ShouldBeAssignableTo<IEnumerable<User>>();
        }

        [Fact]
        public void returns_list_of_users()
        {
            var handler = GetMockedHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText("Files\\users.json"))
            });

            var sut = new JsonService(new HttpClient(handler));

            sut.GetUsers().Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void return_a_httpexception_everytime_the_current_seconds_value_is_divisible_by_5_with_no_remainder()
        {
            var handler = GetMockedHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText("Files\\users.json"))
            });

            var sut = new JsonService(new HttpClient(handler));


            Should.Throw<HttpRequestException>(() => sut.GetUsersWithExceptions(4));

        }
        
        [Fact]
        public void not_return_a_httpexception_everytime_the_current_seconds_value_is_not_divisible_by_5_with_no_remainder()
        {
            var handler = GetMockedHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText("Files\\users.json"))
            });

            var sut = new JsonService(new HttpClient(handler));


            Should.NotThrow(() => sut.GetUsersWithExceptions(3));

        }

        //http://hamidmosalla.com/2017/02/08/mock-httpclient-using-httpmessagehandler/
        private HttpMessageHandler GetMockedHttpMessageHandler(HttpResponseMessage httpResponseMessage)
        {
            var handler = new FakeHttpMessageHandler(httpResponseMessage.StatusCode, httpResponseMessage.Content);

            return handler;
        }


    }

    // http://hamidmosalla.com/2017/02/08/mock-httpclient-using-httpmessagehandler/
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        

        private readonly HttpStatusCode _responseStatusCode;

        private readonly HttpContent _responseContent;
        //public virtual HttpResponseMessage Send(HttpRequestMessage request)
        //{
        //    throw new NotImplementedException("Remember to setup this method with your mocking framework");
        //}

        //protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(Send(request));
        //}

        public FakeHttpMessageHandler(HttpStatusCode responseStatusCode,HttpContent responseContent)
        {
            _responseStatusCode = responseStatusCode;
            _responseContent = responseContent;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            return await Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _responseStatusCode,
                Content = _responseContent
            });
        }


    }


}
