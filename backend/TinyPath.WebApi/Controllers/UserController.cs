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
        return await ProcessRequestAsync(request);
    }
    
    [HttpGet]
    public async Task<ActionResult> ConfirmEmailCommand([FromQuery] ConfirmEmailCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> RegenerateConfirmEmailCommand([FromBody] RegenerateConfirmEmailCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> RefreshTokenCommand()
    {
        var response = await _mediator.Send(new RefreshTokenCommand.Request());

        if (Response.StatusCode == 200)
        {
            DeleteTokenCookie();
            SetTokenCookie(response.Token);
        }

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult> LoginCommand([FromBody] LoginCommand.Request request)
    {
        var response = await _mediator.Send(request);
        SetTokenCookie(response.Token);
        SetTokenCookie(response.RefreshToken, true);
        return Ok(response);
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
           
        return Ok(response);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAuthenticatedUserCommand()
    {
        return await ProcessRequestAsync(new GetAuthenticatedUserCommand.Request());
    }
    
    [HttpPost]
    public async Task<ActionResult> SendEmailToResetPasswordCommand([FromBody] SendEmailToResetPasswordCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> ResetUserPasswordCommand([FromBody] ResetUserPasswordCommand.Request request)
    {
        return await ProcessRequestAsync(request);
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
    
    private async Task<ActionResult> ProcessRequestAsync<TRequest>(TRequest request)
    {
        var response = await _mediator.Send(request!);
        return Ok(response);
    }
}