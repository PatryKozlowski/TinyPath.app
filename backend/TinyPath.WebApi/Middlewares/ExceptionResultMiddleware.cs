using System.Net;
using TinyPath.Application.Exceptions;
using TinyPath.WebApi.Response;

namespace TinyPath.WebApi.Middlewares;

public class ExceptionResultMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionResultMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context, ILogger<ExceptionResultMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (ErrorException e)
        {
            logger.LogDebug($"Error exception {e.Error}");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse { Error = e.Error });
        }
        catch (UnauthorizedException ue)
        {
            logger.LogDebug($"Error exception {ue.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new UnauthorizedResponse { Error = ue.Error ?? "Unauthorized" });
        }
        catch (ValidationException ve)
        {
            logger.LogDebug($"Validation exception {ve.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableContent;
            await context.Response.WriteAsJsonAsync(new ValidationResponse(ve));
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Unhandled exception {ex.Message}");
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorResponse { Error = "Internal server" });
        }
    }
}