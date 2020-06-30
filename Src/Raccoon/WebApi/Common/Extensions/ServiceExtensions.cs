using NSwag;
using Polly;
using System;
using System.Linq;
using NSwag.Generation.Processors.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("ClientAppCorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("WWW-Authenticate");
                        //.WithOrigins(new[] { "https://localhost:3001", "http://localhost:3000" });
                });
            });

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services
                .AddSwaggerDocument(cfg =>
                {
                    cfg.DocumentName = "v1";
                    cfg.ApiGroupNames = new[] { "1" };
                    cfg.IgnoreObsoleteProperties = true;

                    cfg.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "Eknot";
                        document.Info.Description = "Eknot web API";
                    };

                    cfg.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
                    cfg.AddSecurity("JWT token", Enumerable.Empty<string>(),
                        new OpenApiSecurityScheme()
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Description = "Bearer {token}"
                        });
                    });
                // .AddSwaggerDocument() to add more versions

            return services;
        }

        public static IServiceCollection ConfigureHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("SmsService", opt => 
            {
                opt.BaseAddress = new Uri(configuration["RemoteServices:Sms"]);
            })
            .AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)));

            return services;
        }
    }
}
