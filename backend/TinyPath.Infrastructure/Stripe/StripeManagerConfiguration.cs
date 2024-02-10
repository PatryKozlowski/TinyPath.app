using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Stripe;

public static class StripeManagerConfiguration
{
    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        services.AddScoped<IStripeManager, StripeManager>();
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        
        return services;
    }
}