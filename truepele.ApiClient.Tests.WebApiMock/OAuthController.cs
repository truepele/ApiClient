using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using truepele.ApiClient.Tests.WebApiMock.Dto;

namespace truepele.ApiClient.Tests.WebApiMock
{
    [Route("oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly OAuthCredentials[] _credentials;

        public OAuthController(IAuthService authService, IOptions<AppSettings> settings)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _credentials = settings?.Value?.OAuthCredentials ?? throw new ArgumentNullException(nameof(settings));
        }
        
        
        public async Task<IActionResult> Post(OAuthRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.Scope)
                || string.IsNullOrEmpty(requestDto.ClientId)
                || string.IsNullOrEmpty(requestDto.ClientSecret))
            {
                return BadRequest();
            }
            

            if (!_credentials.Any(c => c.Scope == requestDto.Scope
                                       && c.ClientId == requestDto.ClientId
                                       && c.ClientSecret == requestDto.ClientSecret))
            {
                return Forbid();
            }
            

            var token = Guid.NewGuid().ToString();
            await _authService.RegisterTokenAsync(token);
            
            return Ok(new
            {
                access_token = token
            });
        }
    }
}