using HelloAspNet.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathematicsHelper;
using Microsoft.AspNetCore.Http;
using HelloAspNet.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HelloAspNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection
            //services.AddSingleton<ICalculator, Calculator>();
            services.AddMathematicsHelpers();

            services.AddSingleton<IClaimsRepository, ClaimsRepository>();

            services.AddControllers();
            services.AddApplicationInsightsTelemetry();
            services.AddSwaggerDocument(doc =>
            {
                doc.DocumentName = "My super API";
            });

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            context.Principal!.Identities.First().AddClaim(new System.Security.Claims.Claim(ClaimTypes.GivenName, "Tom"));
                            context.Principal!.Identities.First().AddClaim(new System.Security.Claims.Claim(ClaimTypes.Role, "Admin"));
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                // Do something with context.Request (e.g. logging)
                Console.WriteLine("Processing...");

                await next();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hello/world"))
                {
                    await context.Response.WriteAsync("Hello World!");
                    return;
                }

                await next();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
