using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Auth;

public class JwtManager : IJwtManager
{
    private readonly JwtOptions _jwtOptions;
    public const string USER_ID_CLAIM = "UserId"; 
    public const string USER_SESSION_ID_CLAIM = "UserSessionId";
    public const string CONFIRMATION_ID_CLAIM = "ConfirmationId";
    private const string IS_REFRESH_TOKEN = "IsRefreshToken";
    
    public JwtManager(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    private SecurityKey GetSecurityKey()
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
        {
            throw new ArgumentException("Secret key is missing");
        }
        
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));
    }
    
    private string GenerateTokenWithClaims(IEnumerable<Claim> claims, bool isRefreshToken, int expirationInMinutes = 0)
    {
        var symmetricSecurityKey = GetSecurityKey();
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        
        var tokenDesc = new SecurityTokenDescriptor()
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes > 0 ? expirationInMinutes : isRefreshToken ? _jwtOptions.RefreshTokenExpirationInMinutes : _jwtOptions.ExpirationInMinutes),
            SigningCredentials = signingCredentials
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDesc);
        
        return tokenHandler.WriteToken(token);
    }

    public string GenerateToken(Guid userId, Guid? sessionId, bool isRefreshToken = false)
    {
        
        var userIdClaim = new Claim(USER_ID_CLAIM, userId.ToString());
        var sessionIdClaim = new Claim(USER_SESSION_ID_CLAIM, sessionId.ToString()!);
        var isRefreshTokenClaim = new Claim(IS_REFRESH_TOKEN, isRefreshToken.ToString());

        List<Claim> claims;

        if (isRefreshToken && sessionId.IsNull())
        {
            claims = new List<Claim> { userIdClaim, isRefreshTokenClaim };
        }
        else
        {
            claims = new List<Claim> { userIdClaim, sessionIdClaim, isRefreshTokenClaim };
        }
        
        return GenerateTokenWithClaims(claims, isRefreshToken);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        
        var symmetricSecurityKey = GetSecurityKey();
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = symmetricSecurityKey
            }, out _);
        }
        catch (SecurityTokenExpiredException)
        {
            throw new ErrorException("TokenExpired");
        }
        catch
        {
            throw new ErrorException("InvalidToken");
        }

        return true;
    }

    public string? GetClaim(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (securityToken is null)
        {
            return null;
        }
        
        var stringClaimValue = securityToken.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        return stringClaimValue;
    }
    
    public string GenerateConfirmationToken(Guid confirmationId)
    {
        var expirationInMinutes = _jwtOptions.ConfirmationTokenExpirationInMinutes;
        var confirmationIdClaim = new Claim(CONFIRMATION_ID_CLAIM, confirmationId.ToString());
        var claims = new List<Claim> { confirmationIdClaim };
        return GenerateTokenWithClaims(claims, false, expirationInMinutes);
    }

    public bool ShouldRegenerateRefreshToken(DateTimeOffset expirationDate)
    {
        TimeSpan timeUntilExpiration = expirationDate - DateTimeOffset.Now;
        return timeUntilExpiration.TotalDays <= 7;
    }
}