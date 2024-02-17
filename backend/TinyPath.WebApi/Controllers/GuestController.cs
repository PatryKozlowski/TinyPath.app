using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Guest;

namespace TinyPath.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GuestController : BaseController
{
    public GuestController(ILogger<GuestController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpGet]
    public async Task<ActionResult> GetGuestCommand()
    {
        return await ProcessRequestAsync(new GetGuestCommand.Request());
    }
    
    private async Task<ActionResult> ProcessRequestAsync<TRequest>(TRequest request)
    {
        var response = await _mediator.Send(request!);
        return Ok(response);
    }
}