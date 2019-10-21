using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Truepele.ApiClient.Auth
{
    public class AuthDelegatingHandler: DelegatingHandler
    {
        private readonly IAuthTokenProvider _tokenProvider;

        public AuthDelegatingHandler(IAuthTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if (auth == null)
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            var token = await _tokenProvider.GetTokenAsync(request, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            token = await _tokenProvider.GetNewTokenAsync(request, cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}