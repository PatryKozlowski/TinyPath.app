using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Hangfire;

namespace TinyPath.Application.Logic.User;

public abstract class DeleteAccountCommand
{
    public class Request : IRequest<Response>
    {
        public required string Code { get; set; }
    }
    
    public class Response
    {
        public bool Success { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IStripeManager _stripeManager;
        private readonly IBackgroundServices _backgroundServices;
        
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider, IStripeManager stripeManager, IBackgroundServices backgroundServices) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
            _stripeManager = stripeManager;
            _backgroundServices = backgroundServices;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (!int.TryParse(request.Code, out int code))
            {
                throw new ErrorException("InvalidCode");
            }
            
            if (user.DeleteCode != code)
            {
                throw new ErrorException("InvalidCode");
            }

            var isActiveSubscriptions = await _dbContext
                .Subscriptions
                .Where(x => x.UserId == user.Id && x.Expires > DateTimeOffset.UtcNow)
                .AnyAsync();

            if (isActiveSubscriptions)
            {
                var subscription = await _dbContext
                    .Subscriptions
                    .Where(x => x.UserId == user.Id && x.Expires > DateTimeOffset.UtcNow)
                    .Select(x => x.SubscriptionId)
                    .FirstOrDefaultAsync();
                
                if (subscription is not null)
                {
                    await _stripeManager.CancelSubscription(subscription);
                }
            }
            
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            var sessionJobId = user.Session.HangfireId;

            _backgroundServices.DeleteScheduledJob(sessionJobId);
            return new Response() { Success = true };
        }
    }
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("CodeRequired")
                .MaximumLength(6).WithMessage("CodeTooLong")
                .Matches("^[0-9]{6}$").WithMessage("InvalidCodeFormatOnlyNumbersAllowed");
        }
    }
}