using System;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Domain.Models.Configuration
{
    public class SwaggerExcludePropertySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            var excludedProperties = context.Type.GetProperties()
                .Where(x => x.GetCustomAttribute<SwaggerIgnorePropertyAttribute>() != null);

            foreach(var excludedProperty in excludedProperties)
            {
                var propertyToRemove = schema.Properties.Keys.SingleOrDefault(x => string.Equals(x, excludedProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (propertyToRemove != null)
                    schema.Properties.Remove(propertyToRemove);
            }
        }
    }
}
