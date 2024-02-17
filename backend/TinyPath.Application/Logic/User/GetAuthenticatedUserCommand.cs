using MediatR;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.User;

public abstract class GetAuthenticatedUserCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public required string Email { get; set; }
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
                throw new ErrorException("UserNotFound");
            }

            return new Response() { Email = user.Email };
        }
    }
}