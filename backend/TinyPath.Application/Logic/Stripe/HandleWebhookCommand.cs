using MediatR;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Stripe;

public abstract class HandleWebhookCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IStripeManager _stripeManager;
        
        public Handler(IApplicationDbContext dbContext, IStripeManager stripeManager) : base(dbContext)
        {
            _stripeManager = stripeManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await _stripeManager.HandleWebhook();
            return new Response();
        }
    }
}