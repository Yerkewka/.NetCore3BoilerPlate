using Resources;
using Application;
using Persistence;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Common.Extensions;
using Infrastructure;

namespace WebApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddResources()
                .AddApplication()
                .AddInfrastructure()
                .AddPersistence(Configuration);

            services.AddControllers();
            services.AddLocalization(opt => opt.ResourcesPath = "Resources");

            services
                .ConfigureCors()
                .ConfigureHttpClients(Configuration)
                .ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRequestLocalization(opt =>
            {
                opt.DefaultRequestCulture = new RequestCulture("ru-RU");
                opt.SupportedCultures = new[] {
                    new CultureInfo("en"),
                    new CultureInfo("ru"),
                    new CultureInfo("kk")
                };
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCustomExceptionHandler();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
