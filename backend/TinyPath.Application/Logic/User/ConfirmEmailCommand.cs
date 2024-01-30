using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Conformation;

namespace TinyPath.Application.Logic.User;

public abstract class ConfirmEmailCommand
{
    public class Request : IRequest<Response>
    {
        public required string ConfirmEmailCode { get; init; } = string.Empty;
    }
    
    public class Response
    {
        public required string Message { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IAuthDataProvider _authDataProvider;
        
        public Handler(IApplicationDbContext dbContext, IJwtManager jwtManager, IAuthDataProvider authDataProvider) : base(dbContext)
        {
            _authDataProvider = authDataProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var emailConfirmationCodeFromToken = _authDataProvider.GetConfirmationCode(request.ConfirmEmailCode);

            if (emailConfirmationCodeFromToken is null)
            {
                throw new ErrorException("InvalidConfirmationToken");
            }
            
            var user = await _dbContext.Users
                .Include(u => u.EmailConfirmation)
                .FirstOrDefaultAsync(x => x.EmailConfirmation.Code == emailConfirmationCodeFromToken);

            if (user is null)
            {
                throw new ErrorException("InvalidConfirmationCode");
            }
            
            if (user.EmailConfirmation.Expires < DateTimeOffset.Now)
            {
                throw new ErrorException("ConfirmationCodeExpired");
            }
            
            if (user.EmailConfirmed is false && user.EmailConfirmation.Active)
            {
                user.EmailConfirmed = true;
                user.EmailConfirmation.Active = false;
                
                _dbContext.Users.Update(user);
                
                await _dbContext.SaveChangesAsync();
                
                return new Response { Message = "EmailConfirmed" };
            }

            throw new ErrorException("EmailAlreadyConfirmed");
        }
    }
}