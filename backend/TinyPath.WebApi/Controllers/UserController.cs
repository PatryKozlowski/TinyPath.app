using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TinyPath.Application.Logic.User;
using TinyPath.WebApi.Application.Auth;
using TinyPath.WebApi.Response;

namespace TinyPath.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : BaseController
{
    private readonly CookieSettings? _cookieSettings;
    
    public UserController(ILogger<UserController> logger, IMediator mediator, IOptions<CookieSettings> cookieSettings) : base(logger, mediator)
    {
        _cookieSettings = cookieSettings?.Value;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateUserCommand([FromBody] CreateUserCommand.Request request)
    {
        var response = await _mediator.Send(request);
        return Ok(new Message() { Text = response.Message });
    }
    
    [HttpGet]
    public async Task<ActionResult> ConfirmEmailCommand([FromQuery] ConfirmEmailCommand.Request request)
    {
        var response = await _mediator.Send(request);
        return Ok(new Message() { Text = response.Message });
    }
    
    [HttpPost]
    public async Task<ActionResult> RegenerateConfirmEmailCommand([FromBody] RegenerateConfirmEmailCommand.Request request)
    {
        var response = await _mediator.Send(request);
        return Ok(new Message() { Text = response.Message });
    }
    
    [HttpPost]
    public async Task<ActionResult> RefreshTokenCommand()
    {
        var response = await _mediator.Send(new RefreshTokenCommand.Request());

        if (Response.StatusCode == 200)
        {
            DeleteTokenCookie(true);
            SetTokenCookie(response.Token, true);
        }

        return Ok(new JwtResponse() { RefreshToken = response.Token });
    }
    
    [HttpPost]
    public async Task<ActionResult> LoginCommand([FromBody] LoginCommand.Request request)
    {
        var response = await _mediator.Send(request);
        SetTokenCookie(response.Token);
        SetTokenCookie(response.RefreshToken, true);
        return Ok(new JwtResponse() { AccessToken = response.Token, RefreshToken = response.RefreshToken });
    }
    
    [HttpPost]
    public async Task<ActionResult> Logout()
    {
        var response = await _mediator.Send(new LogoutCommand.Request());
        
        if (Response.StatusCode == 200)
        {
            DeleteTokenCookie();
            DeleteTokenCookie(true);
        }
           
        return Ok(new Message() { Text = response.Message });
    }
    
    private void SetTokenCookie(string token, bool isRefreshToken = false)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddMinutes(30),
            SameSite = SameSiteMode.Lax,
        };

        if (_cookieSettings is not null)
        {
            cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(_cookieSettings.Expires),
                SameSite = cookieOptions.SameSite,
                Secure = cookieOptions.Secure,
            };
        }
            
        Response.Cookies.Append(isRefreshToken ? CookieSettings.COOKIE_REFRESH_TOKEN_NAME : CookieSettings.COOKIE_TOKEN_NAME, token, cookieOptions);
    }
        
    private void DeleteTokenCookie(bool isRefreshToken = false)
    {
        Response.Cookies.Delete(isRefreshToken ? CookieSettings.COOKIE_REFRESH_TOKEN_NAME : CookieSettings.COOKIE_TOKEN_NAME, new CookieOptions()
        {
            HttpOnly = true,
        });
    }
}