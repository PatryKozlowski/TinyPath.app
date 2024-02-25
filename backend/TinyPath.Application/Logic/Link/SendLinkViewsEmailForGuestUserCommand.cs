using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Guest;
using TinyPath.Application.Services.Link;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.Link;

public abstract class SendLinkViewsEmailForGuestUserCommand
{
    public class Request : IRequest<Response>
    {
        public required string Email { get; init; } 
    }
    
    public class Response
    {
        public string Message { get; set; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IGuestManager _guestManager;
        private readonly ILinkManager _linkManager;
        private readonly IGetLinkOptions _getLinkOptions;
        private readonly IEmailSchema _emailSchema;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendLinkViewsEmailForGuestUserCommand> _logger;
        
        public Handler(IApplicationDbContext dbContext, IGuestManager guestManager, ILinkManager linkManager, IEmailSchema emailSchema, IEmailSender emailSender, IGetLinkOptions getLinkOptions, ILogger<SendLinkViewsEmailForGuestUserCommand> logger) : base(dbContext)
        {
            _guestManager = guestManager;
            _linkManager = linkManager;
            _emailSchema = emailSchema;
            _emailSender = emailSender;
            _getLinkOptions = getLinkOptions;
            _logger = logger;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var guestUser = await _guestManager.GetGuestUser();
            
            if (guestUser is null)
            {
                throw new ErrorException("GuestUserNotFound");
            }
            
            var lastLinkId = await _linkManager.GetLastGuestUserLinkId(guestUser.Id);
            
            
            if (lastLinkId == Guid.Empty)
            {
                throw new ErrorException("LinkNotFound");
            }
            
            // var host = _getLinkOptions.GetShortLinkHost();
            var host = "http://localhost:3000";
            var linkViewsCount = host + "/visitors?linkId=" + lastLinkId;

            var emailSchema = _emailSchema.GetSchema(EmailSchemas.LinksCountEmail, linkViewsCount);

            try
            {
                await _emailSender.SendEmailAsync(request.Email, emailSchema.subject, emailSchema.content);

                return new Response() { Message = "EmailSent" };
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