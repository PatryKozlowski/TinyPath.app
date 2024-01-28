using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Infrastructure.Persistence;

namespace TinyPath.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration.GetConnectionString("TinyPathDB")!);
        services.AddDbCache();
        
        return services;
    }
}