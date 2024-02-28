using MediatR;
using Microsoft.AspNetCore.Mvc;
using TinyPath.Application.Logic.Stripe;

namespace TinyPath.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class StripeController : BaseController
{
    public StripeController(ILogger<StripeController> logger, IMediator mediator) : base(logger, mediator)
    {
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionCommand.Request request)
    {
        return await ProcessRequestAsync(request);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBillingPortal()
    {
        return await ProcessRequestAsync(new CreateBillingPortalCommand.Request());
    }
    
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var response = await _mediator.Send(new HandleWebhookCommand.Request());
        return Ok(response);
    }
    
    private async Task<ActionResult> ProcessRequestAsync<TRequest>(TRequest request)
    {
        var response = await _mediator.Send(request!);
        return Ok(response);
    }
}