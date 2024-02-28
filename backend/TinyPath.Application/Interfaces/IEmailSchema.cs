using TinyPath.Domain.Enums;

namespace TinyPath.Application.Interfaces;

public interface IEmailSchema
{
    (string subject, string content) GetSchema(EmailSchemas emailSchema, string? stringParam = null, int? intParam = null);
}