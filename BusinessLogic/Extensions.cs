using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IRolesService, RolesService>();
        serviceCollection.AddScoped<IRequestService, RequestService>();
        serviceCollection.AddScoped<IRefreshTokenService, RefreshTokenService>();
        serviceCollection.AddScoped<IReportService, ReportService>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        return serviceCollection;
    }
}