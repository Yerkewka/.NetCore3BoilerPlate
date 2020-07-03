using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddUserAccessor()
                .AddIdentityService()
                .AddSmsService()
                .AddResponseCacheService(configuration);

            return services;
        }
    }
}
