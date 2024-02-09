using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Guest;

public static class GuestConfiguration
{
    public static IServiceCollection AddGuestServices(this IServiceCollection services)
    {
        services.AddScoped<IGuestManager, GuestManager>();
        return services;
    }
}