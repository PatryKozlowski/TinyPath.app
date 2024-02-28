using TinyPath.Application.Interfaces;
using TinyPath.Domain.Enums;

namespace TinyPath.Infrastructure.Sender;

public class EmailSchema : IEmailSchema
{
    public  (string subject, string content) GetSchema(EmailSchemas emailSchema, string? stringParam = null, int? intParam = null)
    {
        return emailSchema switch
        {
            EmailSchemas.ConfirmEmail => EmailSenderSchemas.EmailConfirmationSchema(stringParam!),
            EmailSchemas.LinksCountEmail => EmailSenderSchemas.EmailWithLinkViewsCountSchema(stringParam!),
            EmailSchemas.SubscriptionInvoiceEmail => EmailSenderSchemas.EmailWithSubscriptionInvoice(stringParam!),
            EmailSchemas.ResetPasswordEmail => EmailSenderSchemas.EmailWithResetPasswordLink(stringParam!),
            EmailSchemas.DeleteAccountEmail => EmailSenderSchemas.EmailWithDeleteAccountCode(intParam!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(emailSchema), emailSchema, null)
        };
    }
}