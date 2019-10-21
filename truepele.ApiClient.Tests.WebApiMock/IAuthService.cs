using System.Threading.Tasks;

namespace truepele.ApiClient.Tests.WebApiMock
{
    public interface IAuthService
    {
        Task RegisterTokenAsync(string token);
        Task<bool> CheckTokenAsync(string token);
    }
}