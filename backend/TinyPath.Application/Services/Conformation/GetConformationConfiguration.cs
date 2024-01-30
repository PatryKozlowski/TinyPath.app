using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Conformation;

public static class GetConformationConfiguration
{
    public static IServiceCollection AddConformation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConformationOptions>(configuration.GetSection("Confirmation"));
        services.AddScoped<IGetConformationLink, GetConformationLink>();
        return services;
    }
}