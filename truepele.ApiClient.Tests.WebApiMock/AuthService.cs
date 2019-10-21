using System.Collections.Generic;
using System.Threading.Tasks;

namespace truepele.ApiClient.Tests.WebApiMock
{
    public class AuthService : IAuthService
    {
        private readonly HashSet<string> _tokens = new HashSet<string>();
        
        public Task RegisterTokenAsync(string token)
        {
            _tokens.Add(token);
            return Task.CompletedTask;
        }

        public Task<bool> CheckTokenAsync(string token)
        {
            return Task.FromResult(_tokens.Contains(token));
        }
    }
}