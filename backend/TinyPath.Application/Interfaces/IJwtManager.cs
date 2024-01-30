namespace TinyPath.Application.Interfaces;

public interface IJwtManager
{
    string GenerateToken(Guid userId, Guid? sessionId, bool isRefreshToken = false);
    bool ValidateToken(string token);
    string? GetClaim(string token, string claimType);
    string GenerateConfirmationToken(Guid confirmationId);
}