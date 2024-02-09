using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Hangfire;

public static class HangfireServicesConfiguration
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services)
    {
        services.AddScoped<IBackgroundServices, BackgroundServices>();
        return services;
    }
}