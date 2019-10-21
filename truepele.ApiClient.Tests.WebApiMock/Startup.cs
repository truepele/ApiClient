using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace truepele.ApiClient.Tests.WebApiMock
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseDeveloperExceptionPage()
            .Use(async (context, next) =>
                {
                    if (context.Request.Path.HasValue
                        && context.Request.Path.Value.ToLower().EndsWith("oauth"))
                    {
                        await next();
                        return;
                    }

                    string authorization = context.Request.Headers["Authorization"];
                    
                    if (string.IsNullOrEmpty(authorization)
                    || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        await WriteUnauthorizeErrorAsync(context);
                        return;
                    }

                    var token = authorization.Substring("Bearer ".Length).Trim();
                    
                    if (string.IsNullOrEmpty(token))
                    {
                        await WriteUnauthorizeErrorAsync(context);
                        return;
                    }

                    var authService = app.ApplicationServices.GetService<IAuthService>();

                    if (!await authService.CheckTokenAsync(token))
                    {
                        await WriteUnauthorizeErrorAsync(context);
                        return;
                    }

                    await next();
                })
                .UseMvc();
        }

        private static async Task WriteUnauthorizeErrorAsync(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IAuthService, AuthService>();
        }
    }
}