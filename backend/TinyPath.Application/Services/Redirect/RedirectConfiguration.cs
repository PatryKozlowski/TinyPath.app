using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Redirect;

public static class RedirectConfiguration
{
    public static IServiceCollection AddRedirectServices(this IServiceCollection services)
    {
        services.AddScoped<IRedirectManager, RedirectManager>();
        
        return services;
    }
}