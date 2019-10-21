namespace Truepele.ApiClient.Auth.AuthProviders.OAuth
{
    public class ClientCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}