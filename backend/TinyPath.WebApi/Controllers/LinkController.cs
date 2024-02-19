using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Link;

namespace TinyPath.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LinkController : BaseController
{
    public LinkController(ILogger<LinkController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateShortLinkCommand([FromBody] CreateShortLinkCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateCustomShortLinkCommand([FromBody] CreateCustomShortLinkCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateGuestCustomShortLinkCommand([FromBody] CreateGuestShortLinkCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<ActionResult> SendLinkViewsEmailForGuestUserCommand([FromBody] SendLinkViewsEmailForGuestUserCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpGet] 
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
