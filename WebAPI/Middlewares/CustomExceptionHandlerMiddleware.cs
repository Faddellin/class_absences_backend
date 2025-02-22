using Common.DtoModels;
using Newtonsoft.Json;
namespace class_absences_backend.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError("Error Message: {exceptionMessage}, Time: {occurrenceTime}",e.Message,DateTime.Now);
                await CreateExceptionMessage(context, e, 500);
            }

        }

        private static Task CreateExceptionMessage(HttpContext context, Exception ex, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(JsonConvert.SerializeObject(
                new Response
                {
                    Status = statusCode.ToString(),
                    Message = ex.Message
                })
            );

        }
    }


    public static class CustomExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomExtensionsHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }

    }
}
