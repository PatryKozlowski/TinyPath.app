using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Sender;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailSenderOptions _options;

    public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSenderOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = _options.Mail;
        var password = _options.Password;
        var host = _options.Host;
        var port = _options.Port;
        var enableSsl = _options.EnableSSL;
        var displayName = _options.DisplayName;
        
        if (mail is null || password is null || host is null || displayName is null)
        {
            _logger.LogError("EmailSenderOptions is not configured correctly");
            throw new Exception();
        }
        
        var smtClient = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(mail, password)
        };
        
        var mailMessage = new MailMessage(mail, email, subject, message)
        {
            IsBodyHtml = true,
            From = new MailAddress(mail, displayName)
        };
        
        return smtClient.SendMailAsync(mailMessage);
    }
}