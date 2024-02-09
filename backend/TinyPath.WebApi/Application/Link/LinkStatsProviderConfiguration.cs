using TinyPath.Application.Interfaces;

namespace TinyPath.WebApi.Application.Link;

public static class LinkStatsProviderConfiguration
{
    public static IServiceCollection AddLinkStatsProvider(this IServiceCollection services)
    {
        services.AddScoped<ILinkStatsProvider, LinkStatsProvider>();
        return services;
    }
}