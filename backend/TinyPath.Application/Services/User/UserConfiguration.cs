using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.User;

public static class UserConfiguration
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<IUserManager, UserManager>();
        return services;
    }
}