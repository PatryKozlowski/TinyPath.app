using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Infrastructure.Persistence;

public static class TinyPathDbCacheConfiguration
{
    public static IServiceCollection AddDbCache(this IServiceCollection services)
    {
        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider(
                CacheExpirationMode.Absolute,
                TimeSpan.FromMinutes(2))
                .DisableLogging(false)
                .UseCacheKeyPrefix("TinyPath_");
        });
        
        return services;
    }
}