using TinyPath.Application.Interfaces;

namespace TinyPath.WebApi.Application.Auth;

public static class AuthDataProviderConfiguration
{
    public static IServiceCollection AddAuthDataProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));
        services.AddScoped<IAuthDataProvider, AuthDataProvider>();
        return services;
    }
}