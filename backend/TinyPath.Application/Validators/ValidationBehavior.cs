using FluentValidation;
using MediatR;
using ValidationException = TinyPath.Application.Exceptions.ValidationException;

namespace TinyPath.Application.Validators;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IList<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToList();
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        var context = new ValidationContext<TRequest>(request);
        var errors = _validators
            .Select(v => v.Validate(context))
            .SelectMany(v => v.Errors)
            .Where(e => e != null)
            .GroupBy(e => new { e.PropertyName, e.ErrorCode, e.ErrorMessage })
            .ToList();

        if (errors.Any())
        {
            throw new Exceptions.ValidationException()
            {
                Errors = errors.Select(e => new ValidationException.FieldError()
                {
                    Error = e.Key.ErrorCode,
                    FieldName = e.Key.PropertyName,
                    ErrorMessage = e.Key.ErrorMessage
                }).ToList()
            };
        }
        return await next();
    }
}