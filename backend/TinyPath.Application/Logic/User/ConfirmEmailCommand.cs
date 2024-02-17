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
        public required string RedirectUrl { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IAuthDataProvider _authDataProvider;
        private readonly IGetConfirmationLink _confirmationLink;
        
        public Handler(IApplicationDbContext dbContext, IAuthDataProvider authDataProvider, IGetConfirmationLink confirmationLink) : base(dbContext)
        {
            _authDataProvider = authDataProvider;
            _confirmationLink = confirmationLink;
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
            
            if (user.EmailConfirmation.Expires < DateTimeOffset.UtcNow)
            {
                throw new ErrorException("ConfirmationCodeExpired");
            }
            
            if (user.EmailConfirmed is false && user.EmailConfirmation.Active)
            {
                user.EmailConfirmed = true;
                user.EmailConfirmation.Active = false;
                
                _dbContext.Users.Update(user);
                
                await _dbContext.SaveChangesAsync();
                
                var redirectUrl = _confirmationLink.RedirectConfirmationLink();
                
                return new Response { Message = "EmailConfirmed", RedirectUrl = redirectUrl };
            }

            throw new ErrorException("EmailAlreadyConfirmed");
        }
    }
}