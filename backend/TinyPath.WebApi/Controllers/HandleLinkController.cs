using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Redirect;
using TinyPath.Application.Services.Link;

namespace TinyPath.WebApi.Controllers;

[ApiController]
public class HandleLinkController : BaseController
{
    public HandleLinkController(ILogger<HandleLinkController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpGet("{linkCode}")]
    public async Task<IActionResult> RedirectedToOriginalLinkCommand([FromRoute] HandleRedirectedToOriginalLinkCommand.Request request)
    {
        var result = await _mediator.Send(request);

        return result.StatusCode switch
        {
            302 => Redirect(result.Link),
            _ => NoContent()
        };
    }
}