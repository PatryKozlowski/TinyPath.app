using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Sender;

public static class EmailSenderConfiguration
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSenderOptions>(configuration.GetSection("EmailSender"));
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddSingleton<IEmailSchema, EmailSchema>();
        return services;
    }
}