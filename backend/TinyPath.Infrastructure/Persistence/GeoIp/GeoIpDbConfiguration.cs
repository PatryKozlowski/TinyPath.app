using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Persistence.GeoIp;

public static class GeoIpDbConfiguration
{
    public static IServiceCollection AddGeoIpDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IGeoIpDbContext ,GeoIpDbContext>(options => options.UseNpgsql(connectionString));
        
        return services;
    }
}