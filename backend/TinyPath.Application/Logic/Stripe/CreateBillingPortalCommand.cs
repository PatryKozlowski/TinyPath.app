using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Stripe;

public abstract class CreateBillingPortalCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public string Link { get; init; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IStripeManager _stripeManager;
        private readonly ICurrentUserProvider _currentUserProvider;
        
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider, IStripeManager stripeManager) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
            _stripeManager = stripeManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();

            if (user is null)
            {
                throw new UnauthorizedException();
            }
            
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            
            if (customer is null)
            {
                throw new ErrorException("UserDoesNotHaveStripeCustomer");
            }
            
            var customerId = customer.CustomerId;
            
            var link = await _stripeManager.CreateBillingPortal(customerId);
            
            return new Response
            {
                Link = link
            };

        }
    }
}