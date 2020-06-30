using Application.Common.Interfaces.Services;
using Infrastructure.Services.Indentity;
using Infrastructure.Services.Sms;
using Infrastructure.Services.UserAccessor;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
