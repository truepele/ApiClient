namespace truepele.ApiClient.Tests.WebApiMock
{
    public class AppSettings
    {
        public OAuthCredentials[] OAuthCredentials { get; set; }
        public int ApiFailRatio { get; set; }
        public int OAuthFailRatio { get; set; }
    }
}