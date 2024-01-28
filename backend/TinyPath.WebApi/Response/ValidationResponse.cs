using TinyPath.Application.Exceptions;

namespace TinyPath.WebApi.Response;

public class ValidationResponse
{
    public ValidationResponse()
    {
        
    }

    public ValidationResponse(ValidationException validationException)
    {
        if (validationException is not null)
        {
            Errors = validationException.Errors.Select(e => new FieldValidationError()
            {
                Error = e.Error,
                FieldName = e.FieldName,
                ErrorMessage = e.ErrorMessage
            }).ToList();
        }
    }
    
    public List<FieldValidationError> Errors { get; set; } = new List<FieldValidationError>();

    public class FieldValidationError
    {
        public required string FieldName { get; set; }
        public required string Error { get; set; }
        public required string ErrorMessage { get; set; }

    }
}