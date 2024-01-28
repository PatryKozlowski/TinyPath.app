using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Persistence;

public static class TinyPathDbConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IApplicationDbContext, TinyPathDbContext>((Action<IServiceProvider, DbContextOptionsBuilder>)ConfigureDbContext);
        
        return services;

        void ConfigureDbContext(IServiceProvider provider, DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());
        }
    }
}