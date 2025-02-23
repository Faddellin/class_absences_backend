using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DtoModels;
using Newtonsoft.Json;
namespace class_absences_backend.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;
        private readonly ITokenService _tokenService;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger, ITokenService tokenService)
        {
            _next = next;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (ArgumentException e)
            {

                await CreateExceptionMessage(context, _logger, _tokenService, e, 400, "BadRequest");
            }
            catch (NullReferenceException e)
            {

                await CreateExceptionMessage(context, _logger, _tokenService, e, 500, "InternalServerError");
            }
            catch (AccessViolationException e)
            {

                await CreateExceptionMessage(context, _logger, _tokenService, e, 403, "Forbidden");
            }
            catch (Exception e)
            {

                await CreateExceptionMessage(context, _logger, _tokenService, e, 500, "InternalServerError");
            }

        }

        private static Task CreateExceptionMessage(HttpContext context, ILogger logger, ITokenService tokenService,
                                                   Exception ex, int statusCode, string errorName)
        {

            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var userId = tokenService.GetUserIdFromToken(token).Result;

            logger.LogError("Error Message: {exceptionMessage}, Time: {occurrenceTime}, User: {userId}", ex.Message, DateTime.Now, userId);


            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(JsonConvert.SerializeObject(
                new Response
                {
                    Status = $"Error: {errorName}",
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
