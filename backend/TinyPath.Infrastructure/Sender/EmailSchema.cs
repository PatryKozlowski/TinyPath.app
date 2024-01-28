using TinyPath.Domain.Enums;

namespace TinyPath.Infrastructure.Sender;

public class EmailSchema
{
    public (string subject, string content) GetSchema(EmailSchemas emailSchema, string? stringParam = null)
    {
        return emailSchema switch
        {
            EmailSchemas.ConfirmEmail => EmailSenderSchemas.EmailConfirmationSchema(stringParam!),
            EmailSchemas.LinksCountEmail => EmailSenderSchemas.EmailWithLinkViewsCountSchema(stringParam!),
            _ => throw new ArgumentOutOfRangeException(nameof(emailSchema), emailSchema, null)
        };
    }
}