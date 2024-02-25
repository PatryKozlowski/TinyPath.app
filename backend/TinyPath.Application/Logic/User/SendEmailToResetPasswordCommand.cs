using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Conformation;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.User;

public abstract class SendEmailToResetPasswordCommand
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
        private readonly IGetConfirmationLink _confirmationLink;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSchema _emailSchema;
        private readonly ILogger<SendEmailToResetPasswordCommand> _logger;
        
        public Handler(IApplicationDbContext dbContext, IJwtManager jwtManager, IGetConfirmationLink confirmationLink, IEmailSchema emailSchema, IEmailSender emailSender, ILogger<SendEmailToResetPasswordCommand> logger) : base(dbContext)
        {
            _jwtManager = jwtManager;
            _confirmationLink = confirmationLink;
            _emailSchema = emailSchema;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            
            if (user is null)
            {
                throw new ErrorException("UserNotFound");
            }
            
            var resetPasswordToken = _jwtManager.GenerateConfirmationToken(user.Id);
            
            var confirmationLink = _confirmationLink.EmailConfirmationResetPasswordLink(resetPasswordToken);
            
            var email = _emailSchema.GetSchema(EmailSchemas.ResetPasswordEmail, confirmationLink);
            
            try
            {
                await _emailSender.SendEmailAsync(request.Email, email.subject, email.content);
                return new Response { Message = "EmailSent" };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ErrorSendingEmail");
                throw new Exception("Internal Server");
            }
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