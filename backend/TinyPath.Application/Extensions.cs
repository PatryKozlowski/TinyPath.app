using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Services.Conformation;
using TinyPath.Application.Validators;
using Microsoft.Extensions.Configuration;

namespace TinyPath.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConformation(configuration);
        services.AddValidators();
        
        return services;
    }
}