using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Link;

namespace TinyPath.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]

public class LinkController : BaseController
{
    public LinkController(ILogger<LinkController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateShortLinkCommand([FromBody] CreateShortLinkCommand.Request request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateCustomShortLinkCommand([FromBody] CreateCustomShortLinkCommand.Request request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}