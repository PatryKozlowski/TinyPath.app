using System.Net;
using TinyPath.Application.Interfaces;
using TinyPath.Infrastructure.Auth;

namespace TinyPath.WebApi.Application.Auth;

public class AuthDataProvider : IAuthDataProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtManager _jwtManager;
    private const string X_CLIENT_IP = "X-Client-IP";
    private const string X_FORWARDED_FOR = "X-Forwarded-For";
    private const string STRIPE_SIGNATURE_HEADER = "Stripe-Signature";
    private const string BEARER = "Bearer";
    private const string AUTHORIZATION = "Authorization";

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

    public string GetRemoteIpAddress()
    {
        var headers = _httpContextAccessor.HttpContext?.Request.Headers;

        if (headers!.ContainsKey(X_CLIENT_IP))
        {
            return headers[X_CLIENT_IP].ToString();
        }
        else if (headers.ContainsKey(X_FORWARDED_FOR))
        {
            var forwardedFor = headers[X_FORWARDED_FOR].ToString();
            var forwardedForSplit = forwardedFor.Split(",");
            
            if (forwardedForSplit.Length > 0 && IPAddress.TryParse(forwardedForSplit[0], out var ipAddress))
            {
                return ipAddress.ToString();
            }
        }

        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()!;
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
        var token = _httpContextAccessor.HttpContext?.Request.Headers[AUTHORIZATION].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        
        var splitToken = token.Split(" ");
        if (splitToken.Length > 1 && splitToken[0].Equals(BEARER))
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
    
    public async Task<string> GetStripeRequestBody()
    {
        var json = await new StreamReader(_httpContextAccessor?.HttpContext?.Request.Body!).ReadToEndAsync();
        
        return json;
    }

    public async Task<string> GetStripeSignatureHeader()
    {
        var stripeSignatureHeader = _httpContextAccessor?.HttpContext?.Request.Headers[STRIPE_SIGNATURE_HEADER];
        return stripeSignatureHeader.ToString()!;
    }
}