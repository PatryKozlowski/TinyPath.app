using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Conformation;
using TinyPath.Domain.Entities;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.User;

public abstract class RegenerateConfirmEmailCommand
{
    public class Request : IRequest<Response>
    {
        public required string Email { get; init; } = string.Empty;
    }
    
    public class Response
    {
        public required string Message { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IJwtManager _jwtManager;
        private readonly IGetJwtOptions _getJwtOptions;
        private readonly IGetConfirmationLink _getConfirmationLink;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSchema _emailSchema;
        
        public Handler(IApplicationDbContext dbContext, IJwtManager jwtManager, IGetJwtOptions getJwtOptions, IGetConfirmationLink getConfirmationLink, IEmailSender emailSender, IEmailSchema emailSchema) : base(dbContext)
        {
            _jwtManager = jwtManager;
            _getJwtOptions = getJwtOptions;
            _getConfirmationLink = getConfirmationLink;
            _emailSender = emailSender;
            _emailSchema = emailSchema;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.EmailConfirmation)
                .Where(x => !x.EmailConfirmed)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
            {
                throw new ErrorException("InvalidEmail");
            }

            if (!user.EmailConfirmation.Active)
            {
                throw new ErrorException("EmailConfirmationNotActive");
            }

            if (user.EmailConfirmation.Expires < DateTimeOffset.UtcNow)
            {

                var emailConfirmationCode = Guid.NewGuid();
                var emailConfirmationToken = _jwtManager.GenerateConfirmationToken(emailConfirmationCode);
                var emailConfirmationTokenExpirationTime = _getJwtOptions.GetExpirationConfirmationTokenTime();

                var emailConfirmation = new EmailConfirmation
                {
                    Code = emailConfirmationCode,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(emailConfirmationTokenExpirationTime),
                    Active = true,
                    UserId = user.Id
                };

                user.EmailConfirmation = emailConfirmation;

                _dbContext.EmailConfirmations.Update(emailConfirmation);

                var activationLink = _getConfirmationLink.EmailConfirmationLink(emailConfirmationToken);

                var emailMessage = _emailSchema.GetSchema(EmailSchemas.ConfirmEmail, activationLink);

                await _emailSender.SendEmailAsync(user.Email, emailMessage.subject, emailMessage.content);
                
                await _dbContext.SaveChangesAsync();

                return new Response { Message = "EmailConfirmationSent" };
            }
            
            throw new ErrorException("ConfirmationCodeNotExpired");
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("EmailIsRequired")
                .EmailAddress().WithMessage("EmailIsNotValid");
        }
    }
}