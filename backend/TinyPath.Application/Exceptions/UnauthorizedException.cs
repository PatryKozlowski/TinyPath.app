namespace TinyPath.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public string? Error { get; private set; }
    
    public UnauthorizedException(string? error = "Unauthorized")
    {
        Error = error;
    }
}