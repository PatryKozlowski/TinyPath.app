using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TinyPath.Application.Services.Conformation;

public static class GetConfirmationConfiguration
{
    public static IServiceCollection AddConformation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfirmatioOptions>(configuration.GetSection("Confirmation"));
        services.AddScoped<IGetConfirmationLink, GetConfirmationLink>();
        return services;
    }
}