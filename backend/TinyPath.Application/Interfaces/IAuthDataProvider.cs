namespace TinyPath.Application.Interfaces;

public interface IAuthDataProvider
{
    Guid? GetConfirmationCode(string confirmationToken);
    string? GetRefreshTokenFromCookie();
    Guid? GetUserId(bool fromRefreshToken = false);
    Guid? GetSessionUserId();
    string GetRemoteIpAddress();
}