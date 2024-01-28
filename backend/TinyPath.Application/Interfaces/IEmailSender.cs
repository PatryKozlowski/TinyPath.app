using TinyPath.Domain.Enums;

namespace TinyPath.Application.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}