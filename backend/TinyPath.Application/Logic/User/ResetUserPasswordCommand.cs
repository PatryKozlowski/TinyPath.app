using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.User;

public abstract class ResetUserPasswordCommand
{
    public class Request : IRequest<Response>
    {
        public required string Token { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
        public required string RepeatPassword { get; init; } = string.Empty;

    }
    
    public class Response
    {
        public required string Message { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IAuthDataProvider _authDataProvider;
        private readonly IPasswordManager _passwordManager;
        
        public Handler(IApplicationDbContext dbContext, IAuthDataProvider authDataProvider, IPasswordManager passwordManager) : base(dbContext)
        {
            _authDataProvider = authDataProvider;
            _passwordManager = passwordManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userIdFormToken = _authDataProvider.GetConfirmationCode(request.Token);
            
            if (userIdFormToken is null)
            {
                throw new ErrorException("InvalidConfirmationToken");
            }
            
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userIdFormToken);
            
            if (user is null)
            {
                throw new ErrorException("InvalidConfirmationCode");
            }
            
            var samePassword = _passwordManager.VerifyPassword(user.Password, request.Password);
            
            if (samePassword)
            {
                throw new ErrorException("YouHaveToUseDifferentPassword");
            }
            
            if (request.Password != request.RepeatPassword)
            {
                throw new ErrorException("PasswordsDoNotMatch");
            }
            
            user.Password = _passwordManager.HashPassword(request.Password);
            
            _dbContext.Users.Update(user);
            
            await _dbContext.SaveChangesAsync();
            
            return new Response { Message = "PasswordReset" };
        }
    }
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("TokenIsRequired");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("PasswordIsRequired")
                .MinimumLength(8).WithMessage("PasswordMustBeAtLeast8CharactersLong")
                .Matches("[A-Z]").WithMessage("PasswordMustContainUpperCaseLetter")
                .Matches("[0-9]").WithMessage("PasswordMustContainNumber")
                .Matches("[!@#$%^&*()_+}{\":;'?/>.<,]").WithMessage("PasswordMustContainSpecialCharacter")
                .Equal(x => x.RepeatPassword).WithMessage("PasswordsDoNotMatch");

            RuleFor(x => x.RepeatPassword)
                .NotEmpty().WithMessage("RepeatPasswordIsRequired")
                .MinimumLength(8).WithMessage("RepeatPasswordMustBeAtLeast8CharactersLong")
                .Matches("[A-Z]").WithMessage("RepeatPasswordMustContainUpperCaseLetter")
                .Matches("[0-9]").WithMessage("RepeatPasswordMustContainNumber")
                .Matches("[!@#$%^&*()_+}{\":;'?/>.<,]").WithMessage("RepeatPasswordMustContainSpecialCharacter")
                .Equal(x => x.Password).WithMessage("RepeatPasswordsDoNotMatch");
        }
    }
}