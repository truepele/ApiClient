using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Truepele.ApiClient.Auth.AuthProviders.OAuth
{
    public class ClientCredentialsOAuthTokenProvider : ConcurrentAuthTokenProviderBase
    {
        private readonly HttpClient _client;
        private readonly ClientCredentials _credentials;

        public ClientCredentialsOAuthTokenProvider(HttpClient client, ClientCredentials credentials)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }
        
        
        protected override async Task<string> GetNewTokenConcreteAsync(HttpRequestMessage request, CancellationToken cancellation)
        {
            var form = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _credentials.ClientId},
                {"client_secret", _credentials.ClientSecret},
                {"scope", _credentials.Scope}
            };

            var response = await _client.PostAsync("", new FormUrlEncodedContent(form), cancellation);
            var jsonContent = await response.Content.ReadAsStringAsync();
            
            dynamic dto = JObject.Parse(jsonContent);
            return dto.access_token;
        }
    }
}