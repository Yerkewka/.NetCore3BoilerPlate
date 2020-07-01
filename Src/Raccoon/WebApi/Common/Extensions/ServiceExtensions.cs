using Polly;
using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using WebApi.Common.Options.Swagger;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Models.Configuration.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Models.Configuration;

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

        public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            services
                .AddApiVersioning(opt =>
                {
                    opt.DefaultApiVersion = new ApiVersion(1, 0);
                    opt.ReportApiVersions = true;
                    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddVersionedApiExplorer(opt => 
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    opt.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    opt.SubstituteApiVersionInUrl = true;
                });

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            
            services.AddSwaggerGen(opt =>
            {
                opt.OperationFilter<SwaggerDefaultValues>();
                opt.SchemaFilter<SwaggerExcludePropertySchemaFilter>();

                opt.ExampleFilters();

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using bearer scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                opt.IncludeXmlComments(GetXmlCommentFilePath());
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

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

        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = new JwtOptions();
            configuration.Bind(nameof(JwtOptions), jwtOptions);
            services.AddSingleton(jwtOptions);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParameters);

            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddAuthorization();

            return services;
        }

        private static string GetXmlCommentFilePath()
        {
            var basePath = AppContext.BaseDirectory;
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            
            return Path.Combine(basePath, fileName);
        }
    }
}
