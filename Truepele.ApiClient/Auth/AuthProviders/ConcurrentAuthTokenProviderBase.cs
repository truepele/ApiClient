using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Truepele.ApiClient.Auth.AuthProviders
{
    public abstract class ConcurrentAuthTokenProviderBase : IAuthTokenProvider
    {
        private Task<string> _cachedTokenTask = Task.FromResult("");
        private readonly object _lock = new object();
            
        
        public async Task<string> GetTokenAsync(HttpRequestMessage request, CancellationToken cancellation)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            string token;
            
            try
            {
                token = await _cachedTokenTask;
            }
            catch
            {
                return await GetNewTokenAsync(request, cancellation);
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }
                
            return await GetNewTokenAsync(request, cancellation);
        }
        

        public Task<string> GetNewTokenAsync(HttpRequestMessage request, CancellationToken cancellation)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            var cachedTokenTask = _cachedTokenTask;

            lock (_lock)
            {
                if (_cachedTokenTask != cachedTokenTask)
                {
                    return _cachedTokenTask;
                }

                var newTokenTask = GetNewTokenConcreteAsync(request, cancellation);
                if (newTokenTask == null)
                {
                    throw new InvalidOperationException($"{nameof(GetNewTokenConcreteAsync)} should not return null task");
                }

                return _cachedTokenTask = newTokenTask;
            }
        }
        

        protected abstract Task<string> GetNewTokenConcreteAsync(HttpRequestMessage request, CancellationToken cancellation);
    }
}