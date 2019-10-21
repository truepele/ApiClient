using System;
using Microsoft.Extensions.DependencyInjection;

namespace Truepele.ApiClient.Auth
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddAuthHandler(this IHttpClientBuilder builder, 
            Func<IServiceProvider, IAuthTokenProvider> tokenProviderSelector)
        {
            return builder
                .AddHttpMessageHandler(p => new AuthDelegatingHandler(tokenProviderSelector(p)));
        }
    }
}