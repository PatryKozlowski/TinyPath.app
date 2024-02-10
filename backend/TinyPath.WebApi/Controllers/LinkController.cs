using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Link;

namespace TinyPath.WebApi.Controllers;

[ApiController]
public class LinkController : BaseController
{
    public LinkController(ILogger<LinkController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpPost("api/[controller]/[action]")]
    public async Task<ActionResult> CreateShortLink([FromBody] CreateShortLinkCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost("api/[controller]/[action]")]
    public async Task<ActionResult> CreateCustomShortLink([FromBody] CreateCustomShortLinkCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost("api/[controller]/[action]")]
    public async Task<ActionResult> SendLinkViewsEmailForGuestUser([FromBody] SendLinkViewsEmailForGuestUserCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpGet("[controller]/[action]")] 
    public async Task<ActionResult> GetLinkViewsCountForGuestUser([FromQuery] GetLinkViewsCountForGuestUserCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    private async Task<ActionResult> ProcessRequestAsync<TRequest>(TRequest request)
    {
        var response = await _mediator.Send(request!);
        return Ok(response);
    }
}
