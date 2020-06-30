using MongoDB.Driver;
using MongoDB.Entities;
using Domain.Entities.Indentity;
using AspNetCore.Identity.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default:ConnectionString");
            var databaseName = configuration.GetConnectionString("Default:DatabaseName");

            services.AddMongoDBEntities(MongoClientSettings.FromConnectionString(connectionString), databaseName);

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(mongo =>
            {
                mongo.ConnectionString = connectionString;
            });

            return services;
        }
    }
}
