using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.Services.Indentity;
using Application.Common.Interfaces.Services.Notification;
using Infrastructure.Services.Cache;
using Infrastructure.Services.Cache.Options;
using Infrastructure.Services.Indentity;
using Infrastructure.Services.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserAccessor(this IServiceCollection services)
        {
            return services.AddScoped<IUserAccessor, UserAccessor>();
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            return services.AddScoped<IIdentityService, IdentityService>();
        }

        public static IServiceCollection AddSmsService(this IServiceCollection services)
        {
            return services.AddScoped<ISmsService, SmsService>();
        }

        public static IServiceCollection AddResponseCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheOptions = new RedisCacheOptions();
            configuration.Bind(nameof(RedisCacheOptions), redisCacheOptions);
            services.AddSingleton(redisCacheOptions);

            if (!redisCacheOptions.Enabled)
                return services;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCacheOptions.ConnectionString));
            services.AddStackExchangeRedisCache(opt => opt.Configuration = redisCacheOptions.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            return services;
        }
    }
}
