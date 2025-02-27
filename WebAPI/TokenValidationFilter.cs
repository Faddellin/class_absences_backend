using BusinessLogic.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace class_absences_backend;

public class TokenValidationFilter : IAsyncActionFilter
{
    private readonly ITokenService _tokenService;

    public TokenValidationFilter(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var hasAuthorize = context.ActionDescriptor.EndpointMetadata
            .Any(o => o.GetType() == typeof(AuthorizeAttribute));

        if (hasAuthorize)
        {
            var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token) || !await _tokenService.IsTokenValid(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var userId = await _tokenService.GetUserIdFromToken(token);
            context.HttpContext.Items["userId"] = userId;
        }

        await next();
    }
}