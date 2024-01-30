using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;
using TinyPath.Infrastructure.Auth;
using TinyPath.Infrastructure.Persistence;
using TinyPath.Infrastructure.Sender;

namespace TinyPath.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration.GetConnectionString("TinyPathDB")!);
        services.AddDbCache();
        services.AddAuth(configuration);
        services.AddEmailSender(configuration);
        services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        services.AddScoped<IPasswordManager, PasswordManager>();
        
        return services;
    }
}