using Microsoft.AspNetCore.Mvc;

namespace truepele.ApiClient.Tests.WebApiMock.Dto
{
    public class OAuthRequestDto
    {
        [BindProperty(Name = "client_id")]
        public string ClientId { get; set; }
        
        [BindProperty(Name = "client_secret")]
        public string ClientSecret { get; set; }
        
        public string Scope { get; set; }
    }
}