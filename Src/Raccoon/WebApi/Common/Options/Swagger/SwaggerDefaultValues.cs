using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Common.Options.Swagger
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null)
                return;

            foreach(var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(x => x.Name == parameter.Name);
                if (parameter.Description == null)
                    parameter.Description = description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null || description.DefaultValue != null)
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
