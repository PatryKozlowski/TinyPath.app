namespace TinyPath.WebApi.Response;

public class JwtResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}