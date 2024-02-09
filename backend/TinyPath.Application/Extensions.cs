using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Services.Conformation;
using TinyPath.Application.Validators;
using Microsoft.Extensions.Configuration;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Services;
using TinyPath.Application.Services.Guest;
using TinyPath.Application.Services.Hangfire;
using TinyPath.Application.Services.Link;
using TinyPath.Application.Services.Redirect;

namespace TinyPath.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConformation(configuration);
        services.AddValidators();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddGuestServices();
        services.AddRedirectServices();
        services.AddLinkServices(configuration);
        services.AddHangfireServices();
        
        return services;
    }
}