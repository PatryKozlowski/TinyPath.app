using TinyPath.Application.Interfaces;
using TinyPath.Infrastructure.Auth;

namespace TinyPath.WebApi.Application.Auth;

public class AuthDataProvider : IAuthDataProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtManager _jwtManager;

    public AuthDataProvider(IHttpContextAccessor httpContextAccessor, IJwtManager jwtManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtManager = jwtManager;
    }
    
    public Guid? GetConfirmationCode(string confirmationToken)
    {
        var confirmationCode = GetClaimValue(JwtManager.CONFIRMATION_ID_CLAIM, false, confirmationToken);
        
        if (Guid.TryParse(confirmationCode, out var parsedConfirmationCode))
        {
            return parsedConfirmationCode;
        }
        
        return null;
    }
    
    public Guid? GetUserId(bool fromRefreshToken = false)
    {
        var userId = GetClaimValue(JwtManager.USER_ID_CLAIM, fromRefreshToken);
        
        if (Guid.TryParse(userId, out var parsedUserId))
        {
            return parsedUserId;
        }
        
        return null;
    }

    public Guid? GetSessionUserId()
    {
        var sessionId = GetClaimValue(JwtManager.USER_SESSION_ID_CLAIM);
        
        if (Guid.TryParse(sessionId, out var parsedSessionId))
        {
            return parsedSessionId;
        }
        
        return null;
    }
    
    public string? GetRefreshTokenFromCookie()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies[CookieSettings.COOKIE_REFRESH_TOKEN_NAME];
        
        return token;
    }
    
    private string? GetTokenFromCookie()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies[CookieSettings.COOKIE_TOKEN_NAME];
        
        return token;
    }
    
    private string? GetTokenFromHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        
        var splitToken = token.Split(" ");
        if (splitToken.Length > 1 && splitToken[0].Equals("Bearer"))
        {
            return splitToken[1];
        }
        
        return null;
    }
    
    private string? GetClaimValue(string claimType, bool fromRefreshToken = false, string confirmationToken = "")
    {
        var token = 
            claimType == JwtManager.CONFIRMATION_ID_CLAIM ? confirmationToken :
            fromRefreshToken ? GetRefreshTokenFromCookie() : GetTokenFromCookie() ?? GetTokenFromHeader();
        
        if (!string.IsNullOrWhiteSpace(token) && _jwtManager.ValidateToken(token))
        {
            var claim = _jwtManager.GetClaim(token, claimType);
            return claim;
        }
        
        return null;
    }
}