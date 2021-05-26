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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
