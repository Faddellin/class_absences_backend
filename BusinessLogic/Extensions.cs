using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DtoModels.Reason;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IRequestService, RequestService>();
        serviceCollection.AddScoped<IRefreshTokenService, RefreshTokenService>();
        serviceCollection.AddScoped<IReasonService, ReasonService>();
        serviceCollection.AddSingleton<ITokenService, TokenService>();
        return serviceCollection;
    }
}