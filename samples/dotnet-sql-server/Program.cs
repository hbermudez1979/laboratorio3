using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Backend
{
    // dotnet run --urls="http://+:5000;https://+:5001"
    public class Program
    {
        public static void Main(string[] args)
            =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Program>();
                }).Build().Run();

        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) 
        {        
            services.AddDbContext<AdventureWorksDbContext>(
                options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AdventureWorks")));

            services.AddScoped<AdventureWorksDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/api/hello");
                    return Task.CompletedTask;
                });

                endpoints.MapGet("api/hello", async context =>
                {
                    await context.Response.WriteAsJsonAsync<object[]>(
                        new object[]{
                        "Hello",
                        "World","Heinner Bermudez Araya"
                    });
                });

                endpoints.MapGet("api/products", async context =>
                {
                    var dbContext = context.Request.HttpContext.RequestServices.GetRequiredService<AdventureWorksDbContext>();

                    await context.Response.WriteAsJsonAsync<object[]>(
                        dbContext.Products.Select(p => 
                            new { 
                                Id = p.ProductId, 
                                Name = p.Name, 
                                ListPrice = p.ListPrice 
                            }).ToArray()
                    );
                });
            });
        }
    }
}
