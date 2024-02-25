using TinyPath.Application.Interfaces;
using TinyPath.Domain.Enums;

namespace TinyPath.Infrastructure.Sender;

public class EmailSchema : IEmailSchema
{
    public  (string subject, string content) GetSchema(EmailSchemas emailSchema, string? stringParam = null)
    {
        return emailSchema switch
        {
            EmailSchemas.ConfirmEmail => EmailSenderSchemas.EmailConfirmationSchema(stringParam!),
            EmailSchemas.LinksCountEmail => EmailSenderSchemas.EmailWithLinkViewsCountSchema(stringParam!),
            EmailSchemas.SubscriptionInvoiceEmail => EmailSenderSchemas.EmailWithSubscriptionInvoice(stringParam!),
            EmailSchemas.ResetPasswordEmail => EmailSenderSchemas.EmailWithResetPasswordLink(stringParam!),
            _ => throw new ArgumentOutOfRangeException(nameof(emailSchema), emailSchema, null)
        };
    }
}