using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace truepele.ApiClient.Tests.WebApiMock
{
    public static class WebHostFactory
    {

        public static IWebHost CreateWebHost(AppSettings settings, int httpPort)
        {
            try
            {
                return WebHost.CreateDefaultBuilder()
                    .UseKestrel(o => o.ListenAnyIP(httpPort))
                    .UseStartup<Startup>()
                    .ConfigureAppConfiguration(builder =>
                        builder
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json", true, true)
                            .AddEnvironmentVariables())
                    .ConfigureServices(services =>
                        services.Configure<AppSettings>(s =>
                        {
                            s.ApiFailRatio = settings.ApiFailRatio;
                            s.OAuthCredentials = settings.OAuthCredentials;
                            s.OAuthFailRatio = settings.OAuthFailRatio;
                        }))
                    .Build();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}