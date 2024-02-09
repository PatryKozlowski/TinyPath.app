using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;
using TinyPath.Infrastructure.Auth;
using TinyPath.Infrastructure.Persistence.GeoIp;
using TinyPath.Infrastructure.Persistence.Hangfire;
using TinyPath.Infrastructure.Persistence.TinyPath;
using TinyPath.Infrastructure.Sender;

namespace TinyPath.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration.GetConnectionString("TinyPathDB")!);
        services.AddGeoIpDatabase(configuration.GetConnectionString("GeoIpDB")!);
        services.AddDbCache();
        services.AddAuth(configuration);
        services.AddEmailSender(configuration);
        services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        services.AddScoped<IPasswordManager, PasswordManager>();
        services.AddHangfireConfiguration(configuration);
        
        return services;
    }
}