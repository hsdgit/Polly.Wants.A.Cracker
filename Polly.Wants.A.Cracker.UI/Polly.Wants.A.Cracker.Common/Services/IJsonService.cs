using System.Collections.Generic;
using Polly.Wants.A.Cracker.Common.Model;

namespace Polly.Wants.A.Cracker.Common.Services
{
    public interface IJsonService
    {
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetUsersWithExceptions(int currentSecond);
    }
}