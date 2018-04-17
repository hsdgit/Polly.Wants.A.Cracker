using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Polly.Wants.A.Cracker.Common.Model;

namespace Polly.Wants.A.Cracker.Common.Services
{
    public class JsonService : IJsonService
    {
        private readonly HttpClient _client;

        public JsonService(HttpClient client)
        {
            _client = client;
        }
        public IEnumerable<User> GetUsers()
        {

            var users = _client.GetStringAsync("https://jsonplaceholder.typicode.com/users").Result;

            return JsonConvert.DeserializeObject<IEnumerable<User>>(users);
        }

        public IEnumerable<User> GetUsersWithExceptions(int currentSecond)
        {
            
            if (currentSecond % 4 == 0)
            {
                throw new HttpRequestException();
            }

            var users = _client.GetStringAsync("https://jsonplaceholder.typicode.com/users").Result;

            return JsonConvert.DeserializeObject<IEnumerable<User>>(users);
        }
    }
}