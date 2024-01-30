using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.User;

public abstract class LogoutCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public required string Message { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IAuthDataProvider _authDataProvider;
        
        public Handler(IApplicationDbContext dbContext, IAuthDataProvider authDataProvider) : base(dbContext)
        {
            _authDataProvider = authDataProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userSessionId = _authDataProvider.GetSessionUserId();

            if (userSessionId.HasValue)
            {
                var user = await _dbContext.Users
                    .Include(us => us.Session)
                    .FirstOrDefaultAsync(us => us.Session.Id == userSessionId.Value);

                if (user is not null)
                {
                    _dbContext.Sessions.Remove(user.Session);
                    
                    await _dbContext.SaveChangesAsync();
                    
                    return new Response() { Message = "UserLoggedOut" };
                }
            }

            throw new UnauthorizedException("UserNotLoggedIn");
        }
    }
}