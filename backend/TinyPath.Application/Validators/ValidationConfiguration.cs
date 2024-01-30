using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Validators;

public static class ValidationConfiguration
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(BaseQueryHandler));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}