using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Link;

public static class LinkConfiguration
{
    public static IServiceCollection AddLinkServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LinkOptions>(configuration.GetSection("ShortLink"));
        services.AddScoped<ILinkManager, LinkManager>();
        services.AddScoped<IGetLinkOptions, GetLinkOptions>();
        return services;
    }
}