using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Truepele.ApiClient.Auth;
using Truepele.ApiClient.Auth.AuthProviders.OAuth;
using truepele.ApiClient.Tests.WebApiMock;

namespace truepele.ApiClient.Tests
{
    public class ApiMockTests
    {
        private IWebHost _webHost;
        private const int Port = 7777;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ClientCredentials _credentials;

        public ApiMockTests()
        {
            _credentials = new ClientCredentials
            {
                Scope = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                ClientSecret = Guid.NewGuid().ToString()
            };
            
            var services = new ServiceCollection();
            
            services
                .AddHttpClient("oauth", c => c.BaseAddress = new Uri($"http://localhost:{Port}/oauth"));

            services.AddHttpClient(nameof(ApiMockTests),
                    c => c.BaseAddress = new Uri($"http://localhost:{Port}/api/values"))
                .AddAuthHandler(p =>
                    new ClientCredentialsOAuthTokenProvider(p.GetService<IHttpClientFactory>().CreateClient("oauth"),
                        _credentials));
            
            _clientFactory = services.BuildServiceProvider().GetService<IHttpClientFactory>();
        }


        [TearDown]
        public async Task TearDown()
        {
            if (_webHost == null)
            {
                throw new InvalidOperationException($"{nameof(_webHost)} should not be null");
            }

            await _webHost.StopAsync();
            _webHost = null;
        }
        

        [Test, Order(0)]
        public async Task ApiMock_Works()
        {
            // Arrange

            await StartWebHostAsync(new AppSettings
            {
                ApiFailRatio = 0,
                OAuthCredentials = new[ ] { new OAuthCredentials
                {
                    ClientId = _credentials.ClientId,
                    Scope = _credentials.Scope,
                    ClientSecret = _credentials.ClientSecret
                }}
            });
            
            var client = _clientFactory.CreateClient(nameof(ApiMockTests)); 
                
            
            // Act / Assert
            
            Assert.DoesNotThrowAsync(() => GetAsync(client, ""));
        }


        [Test]
        public async Task ApiMock_Fails()
        {
            // Arrange

            await StartWebHostAsync(new AppSettings
            {
                ApiFailRatio = 2,
                OAuthCredentials = new[ ] { new OAuthCredentials
                {
                    ClientId = _credentials.ClientId,
                    Scope = _credentials.Scope,
                    ClientSecret = _credentials.ClientSecret
                }}
            });
            var client = _clientFactory.CreateClient(nameof(ApiMockTests));
            

            // Act / Assert
            
            Assert.ThrowsAsync<HttpRequestException>(() => GetAsync(client, ""));
            Assert.DoesNotThrowAsync(() => GetAsync(client, ""));
            Assert.ThrowsAsync<HttpRequestException>(() => GetAsync(client, ""));
        }
        

        private Task StartWebHostAsync(AppSettings settings)
        {
            _webHost = WebHostFactory.CreateWebHost(settings, Port);
            return _webHost.StartAsync();
        }
        

        private static async Task<string> GetAsync(HttpClient client, string requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer");
            
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}