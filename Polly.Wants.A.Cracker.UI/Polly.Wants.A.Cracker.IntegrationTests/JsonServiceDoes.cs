using System;
using System.Linq;
using System.Net.Http;
using Polly.Wants.A.Cracker.Common.Services;
using Shouldly;
using Xunit;

namespace Polly.Wants.A.Cracker.IntegrationTests
{
    public class JsonServiceDoes
    {
        [Fact]
        public void returns_users_when_integrated()
        {
            // substitute in IoC in real world scenario
            IJsonService jsonService = new JsonService(new HttpClient());

            jsonService.GetUsers().Count().ShouldBeGreaterThan(0);
        }
    }
}
