using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Auth;

public static class JwtConfiguration
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Authentication"));
        services.AddScoped<IJwtManager, JwtManager>();
        services.AddScoped<IGetJwtOptions, GetJwtOptions>();
        return services;
    }
}