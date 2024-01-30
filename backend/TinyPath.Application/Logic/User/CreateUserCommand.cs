using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Conformation;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.User;

public abstract class CreateUserCommand
{
    public class Request : IRequest<Response>
    {
        public required string Email { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
        public required string RepeatPassword { get; init; } = string.Empty;
    }
    
    public class Response
    {
        public string Message { get; init; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IPasswordManager _passwordManager;
        private readonly IJwtManager _jwtManager;
        private readonly IGetJwtOptions _getJwtOptions;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSchema _emailSchema;
        private readonly IGetConformationLink _conformationLink;
        
        public Handler(IApplicationDbContext dbContext, IPasswordManager passwordManager, IJwtManager jwtManager, IGetJwtOptions getJwtOptions, IEmailSender emailSender, IEmailSchema emailSchema, IGetConformationLink conformationLink) : base(dbContext)
        {
            _passwordManager = passwordManager;
            _jwtManager = jwtManager;
            _getJwtOptions = getJwtOptions;
            _emailSender = emailSender;
            _emailSchema = emailSchema;
            _conformationLink = conformationLink;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
           if (request.Password != request.RepeatPassword)
           {
               return new Response {Message = "PasswordsDoNotMatch"};
           }
           
           var user =  await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

           if (user is not null)
           {
               throw new Exception("EmailAlreadyExists");
           }
           
           var createdUser = new Domain.Entities.User
           {
               Email = request.Email,
               Password = "",
               Role = UserRole.User,
           };
           
           createdUser.Password = _passwordManager.HashPassword(request.Password);
           
           _dbContext.Users.Add(createdUser);
           
           var emailConfirmationCode = Guid.NewGuid();
           var emailConfirmationToken = _jwtManager.GenerateConfirmationToken(emailConfirmationCode);
           var emailConfirmationTokenExpirationTime = _getJwtOptions.GetExpirationConfirmationTokenTime();
           
           var emailConfirm = new Domain.Entities.EmailConfirmation()
           {
               Code = emailConfirmationCode,
               Expires = DateTime.UtcNow.AddMinutes(emailConfirmationTokenExpirationTime),
           }; 
           
           createdUser.EmailConfirmation = emailConfirm;

           _dbContext.EmailConfirmations.Update(emailConfirm);
           
           var emailConfirmationLink = _conformationLink.EmailConfirmationLink(emailConfirmationToken);

           var emailConfirmationSchema = _emailSchema.GetSchema(EmailSchemas.ConfirmEmail, emailConfirmationLink);
           
            await _emailSender.SendEmailAsync(createdUser.Email, emailConfirmationSchema.subject,
                emailConfirmationSchema.content);

            await _dbContext.SaveChangesAsync();

            return new Response() { Message = "UserCreatedConfirmYourEmail" };
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

}