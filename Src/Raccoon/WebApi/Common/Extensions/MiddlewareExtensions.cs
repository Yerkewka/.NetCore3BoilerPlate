using Microsoft.AspNetCore.Builder;
using WebApi.Common.Middlewares;

namespace WebApi.Common.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<CustomExceptionHandler>();
        }
    }
}
