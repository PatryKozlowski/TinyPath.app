using System.Diagnostics.Tracing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.User;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.User;

public abstract class SendEmailWithDeleteAccountCodeCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public bool Success { get; set; }
        public bool IsActiveSubscriptions { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSchema _emailSchema;
        private readonly ILogger<SendEmailWithDeleteAccountCodeCommand> _logger;
        
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider, IEmailSchema emailSchema, IEmailSender emailSender, IUserManager userManager, ILogger<SendEmailWithDeleteAccountCodeCommand> logger) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
            _emailSchema = emailSchema;
            _emailSender = emailSender;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }
            
            var isPremium = await _dbContext
                .Subscriptions
                .Where(x => x.UserId == user.Id && x.Expires > DateTimeOffset.UtcNow)
                .AnyAsync();
            
            var code = _userManager.GenerateDeleteCode();

            try
            {
                var email = _emailSchema.GetSchema(EmailSchemas.DeleteAccountEmail, null, code);
                await _emailSender.SendEmailAsync(user.Email, email.subject, email.content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while sending email with delete account code");
            }
            
            user.DeleteCode = code;
            
            _dbContext.Users.Update(user);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return new Response() { Success = true, IsActiveSubscriptions = isPremium };
        }
    }
}