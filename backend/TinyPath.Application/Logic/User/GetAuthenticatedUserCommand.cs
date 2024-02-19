using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.User;

public abstract class GetAuthenticatedUserCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public required string Email { get; set; }
        public required bool IsSubscribed { get; set; }
        public required bool IsAdmin { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();
            
            if (user is null)
            {
                throw new UnauthorizedException("UserNotFound");
            }

            var subscription = await _dbContext
                .Subscriptions
                .Where(x => x.UserId == user.Id && x.Expires > DateTimeOffset.UtcNow)
                .AnyAsync();
            
            var isAdmin = await _dbContext
                .Users
                .Where(x => x.Id == user.Id && x.Role == UserRole.Admin)
                .AnyAsync();

            return new Response() { Email = user.Email, IsSubscribed = subscription, IsAdmin = isAdmin};
        }
    }
}