using Microsoft.Extensions.DependencyInjection;

namespace Resources
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddResources(this IServiceCollection services)
        {
            services.AddSingleton<IAppResource, SharedResource>();

            return services;
        }
    }
}
