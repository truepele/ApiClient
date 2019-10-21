using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Truepele.ApiClient.Auth
{
    public interface IAuthTokenProvider
    {
        Task<string> GetTokenAsync(HttpRequestMessage request, CancellationToken cancellation);
        
        Task<string> GetNewTokenAsync(HttpRequestMessage request, CancellationToken cancellation);
    }
}