using Application.Common.Exceptions;
using Domain.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Common.Middlewares
{
    public class CustomExceptionHandler
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<CustomExceptionHandler> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, logger);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception, ILogger<CustomExceptionHandler> logger)
        {
            var code = HttpStatusCode.InternalServerError;
            string result;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new ErrorResponse
                    {
                        Message = validationException.Message,
                        Errors = validationException.Failures
                    });
                    break;
                case RestException restException:
                    code = restException.Code;
                    result = JsonConvert.SerializeObject(new ErrorResponse
                    {
                        Message = restException.Message
                    });
                    break;
                default:
                    logger.LogError(exception, "Unhandled exception occured");
                    result = JsonConvert.SerializeObject(new ErrorResponse
                    {
                        Message = exception.Message
                    });
                    break;
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;

            return httpContext.Response.WriteAsync(result);
        }
    }
}
