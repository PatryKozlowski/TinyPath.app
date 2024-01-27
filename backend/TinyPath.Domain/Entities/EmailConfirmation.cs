using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class EmailConfirmation : DomainEntity
{
    public required Guid Code { get; set; }
    public required Guid UserId { get; set; }
    public required DateTimeOffset Expires { get; set; }
    public bool Active { get; set; } = true;
}