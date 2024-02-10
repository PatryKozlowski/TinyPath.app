using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Stripe;

public abstract class CreateCheckoutSessionCommand
{
    public class Request : IRequest<Response>
    {
        public required string PriceCode { get; init; } 
    }
    
    public class Response
    {
        public string Link { get; init; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IStripeManager _stripeManager;
        private readonly ICurrentUserProvider _currentUserProvider;
        
        public Handler(IApplicationDbContext dbContext, IStripeManager stripeManager, ICurrentUserProvider currentUserProvider) : base(dbContext)
        {
            _stripeManager = stripeManager;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();

            if (user is null)
            {
                throw new UnauthorizedException();
            }

            var isActiveSubscription = await _dbContext
                .Subscriptions
                .Where(x => x.UserId == user.Id)
                .Where(x => x.Expires > DateTime.UtcNow)
                .AnyAsync();

            if (isActiveSubscription)
            {
                throw new ErrorException("UserAlreadyHasActiveSubscription");
            }
            
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);

            var stripeCustomerId = string.Empty;
            
            if (customer is null)
            {
                var newCustomer = await _stripeManager.CreateCustomer(user.Email);

                stripeCustomerId = newCustomer.Id;
            }

            var customerId = customer?.CustomerId ?? stripeCustomerId;

            var checkoutSessionLink =
                await _stripeManager.CreateCheckoutSession(customerId!, request.PriceCode);

            return new Response() { Link = checkoutSessionLink };
        }
    }
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PriceCode)
                .NotEmpty().WithMessage("PriceCodeIsRequired");
        }
    }
}