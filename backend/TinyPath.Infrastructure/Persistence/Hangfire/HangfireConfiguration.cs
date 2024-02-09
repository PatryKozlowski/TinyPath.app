using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Services.Link;

namespace TinyPath.Infrastructure.Persistence.Hangfire;

public static class HangfireConfiguration
{
    public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireDbConnectionString = configuration.GetSection("ConnectionStrings")["HangfireConnection"];
        
        services.AddHangfire(c => c
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(hangfireDbConnectionString, new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(15),
                InvisibilityTimeout = TimeSpan.FromMinutes(30),
                DistributedLockTimeout = TimeSpan.FromMinutes(10),
                TransactionSynchronisationTimeout = TimeSpan.FromMilliseconds(500),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                SchemaName = "hangfire",
                AllowUnsafeValues = false,
                UseNativeDatabaseTransactions = true,
                PrepareSchemaIfNecessary = true,
                EnableTransactionScopeEnlistment = true,
                DeleteExpiredBatchSize = 1000
            }));
        
        services.AddHangfireServer();

        return services;
    }
    
    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
    {
        var login = configuration.GetSection("Hangfire")["Login"];
        var password = configuration.GetSection("Hangfire")["Password"];
        var dashboardPath = configuration.GetSection("Hangfire")["Dashboard"];
        
        var options = new DashboardOptions
        {
            Authorization = new[]
            {
                new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    SslRedirect = false,
                    RequireSsl = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = login,
                            PasswordClear = password
                        }
                    }
                })
            },
            AppPath = dashboardPath
        };
        
        app.UseHangfireDashboard(dashboardPath, options);
        
        UseHangfireRecurringJob();
        
        return app;
    }
    
    private static void UseHangfireRecurringJob()
    {
        RecurringJob
            .AddOrUpdate<ILinkManager>("AggregateLinkStats",  x =>  x.AggregateLinkStats(), Cron.Minutely);
    }
}