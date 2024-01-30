namespace TinyPath.Infrastructure.Auth;

public class JwtOptions
{
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpirationInMinutes { get; init; }
    public required int RefreshTokenExpirationInMinutes { get; init; }
    public required int ConfirmationTokenExpirationInMinutes { get; init; }
    public required int RegenerateRefreshTokenOnLoginBeforeExpirationInDays { get; init; }
}