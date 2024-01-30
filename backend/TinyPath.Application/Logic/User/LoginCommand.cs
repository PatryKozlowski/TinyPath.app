using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.User;

public abstract class LoginCommand
{
    public class Request : IRequest<Response>
    {
        public required string Email { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
    }
    
    public class Response
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IPasswordManager _passwordManager;
        private readonly IGetJwtOptions _getJwtOptions;
        private readonly IJwtManager _jwtManager;
        
        public Handler(IApplicationDbContext dbContext, IPasswordManager passwordManager, IGetJwtOptions getJwtOptions, IJwtManager jwtManager) : base(dbContext)
        {
            _passwordManager = passwordManager;
            _getJwtOptions = getJwtOptions;
            _jwtManager = jwtManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(us => us.Session)
                .Include(urf => urf.RefreshToken)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user is null)
            {
                throw new ErrorException("InvalidCredentials");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedException("EmailNotConfirmed");
            }

            if (user.Blocked)
            {
                throw new UnauthorizedException("UserIsBlocked");
            }
            
            if (user.Session is not null && user.Session.Expires > DateTimeOffset.Now)
            {
                throw new UnauthorizedException("UserAlreadyLoggedIn");
            }

            if (!user.RefreshToken.Active)
            {
                throw new UnauthorizedException("RefreshTokenIsNotActive");
            }
            
            if (!_passwordManager.VerifyPassword(user.Password, request.Password))
            {
                throw new ErrorException("InvalidCredentials");
            }

            var sessionUserExpires = _getJwtOptions.GetExpirationTokenTime();

            if (user.Session is null)
            {
                user.Session = new Domain.Entities.Session
                {
                    Expires = DateTimeOffset.Now.AddMinutes(sessionUserExpires),
                    User = user,
                };
                
                _dbContext.Sessions.Add(user.Session);
            }
            else
            {
                user.Session.Expires = DateTimeOffset.Now.AddMinutes(sessionUserExpires);
                _dbContext.Sessions.Update(user.Session);
            }
            
            if (user.RefreshToken is null  || _jwtManager.ShouldRegenerateRefreshToken(user.RefreshToken.Expires))
            {
                var refreshTokenExpirationTime = _getJwtOptions.GetExpirationTokenTime(true);
                var refreshTokenCode = _jwtManager.GenerateToken(user.Id, null, true);
                user.RefreshToken = new Domain.Entities.RefreshToken
                {
                    Token = refreshTokenCode,
                    Expires = DateTimeOffset.Now.AddMinutes(refreshTokenExpirationTime),
                };
                
                _dbContext.RefreshTokens.Add(user.RefreshToken);
            }
            
            var accessToken = _jwtManager.GenerateToken(user.Id, user.Session.Id);
            
            await _dbContext.SaveChangesAsync();
            
            return new Response { Token = accessToken, RefreshToken = user.RefreshToken.Token};
        }
    }
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("EmailIsRequired")
                .EmailAddress().WithMessage("EmailIsNotValid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("PasswordIsRequired")
                .MinimumLength(8).WithMessage("PasswordMustBeAtLeast8CharactersLong")
                .Matches("[A-Z]").WithMessage("PasswordMustContainUpperCaseLetter")
                .Matches("[0-9]").WithMessage("PasswordMustContainNumber")
                .Matches("[!@#$%^&*()_+}{\":;'?/>.<,]").WithMessage("PasswordMustContainSpecialCharacter");
        }
    }
}