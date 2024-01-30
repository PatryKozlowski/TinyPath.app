using Microsoft.Extensions.Options;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Auth;

public class GetJwtOptions :IGetJwtOptions
{
    private readonly JwtOptions _jwtOptions;

    public GetJwtOptions(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public int GetExpirationTokenTime(bool isRefreshToken = false)
    {
        return isRefreshToken ? _jwtOptions.RefreshTokenExpirationInMinutes : _jwtOptions.ExpirationInMinutes;
    }

    public int GetExpirationConfirmationTokenTime()
    {
        return _jwtOptions.ConfirmationTokenExpirationInMinutes;
    }

    public int GetRegenerateRefreshTokenOnLoginBeforeExpirationInDays()
    {
        return _jwtOptions.RegenerateRefreshTokenOnLoginBeforeExpirationInDays;
    }
}