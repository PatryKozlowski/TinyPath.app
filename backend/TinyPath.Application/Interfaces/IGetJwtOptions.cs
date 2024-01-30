namespace TinyPath.Application.Interfaces;

public interface IGetJwtOptions
{
    int GetExpirationTokenTime(bool isRefreshToken = false);
    int GetExpirationConfirmationTokenTime();
    int GetRegenerateRefreshTokenOnLoginBeforeExpirationInDays();
}