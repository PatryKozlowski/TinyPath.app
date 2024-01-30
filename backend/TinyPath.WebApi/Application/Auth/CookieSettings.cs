namespace TinyPath.WebApi.Application.Auth;

public class CookieSettings
{
    public const string COOKIE_TOKEN_NAME = "auth.token";
    public const string COOKIE_REFRESH_TOKEN_NAME = "auth.refresh.token";
    public bool Secure { get; set; } = true;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
    public int Expires { get; set; }
}