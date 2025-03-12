using BusinessLogic;
using BusinessLogic.ServiceInterfaces;
using Common.DtoModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

namespace class_absences_backend.Middlewares;

public class VerifiedStaticFilesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public VerifiedStaticFilesMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/static/images/reasons"))
        {
            using var scope = _serviceProvider.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var userId = await tokenService.GetUserIdFromToken(token);
            var user = await appDbContext.Users.FindAsync(userId);
            
            if (user == null)
            {
                throw new KeyNotFoundException("User was not found");
            }

            var userRoles = user.UserTypes;

            var fileName = Path.GetFileName(context.Request.Path);
            var request = await appDbContext.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Images.Any(i => i.Contains(fileName)));
            if (request == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (userRoles.Contains(UserType.Student) && !userRoles.Contains(UserType.Dean) && 
                !userRoles.Contains(UserType.Admin) && request.User.Id != userId ||
                userRoles.Contains(UserType.Teacher) && !userRoles.Contains(UserType.Dean) && 
                !userRoles.Contains(UserType.Admin) && request.User.Id != userId )
            {
                context.Response.StatusCode = 403;
                return;
            }
        }

        await _next(context);
    }
}

public static class VerifiedStaticFilesMiddlewareExtension
{
    public static IApplicationBuilder UseVerifiedStaticFiles(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VerifiedStaticFilesMiddleware>();
    }

}
