namespace TinyPath.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public string? Reason { get; private set; }
    
    public UnauthorizedException(string? reason = "Unauthorized")
    {
        Reason = reason;
    }
}