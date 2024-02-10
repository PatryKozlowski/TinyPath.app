using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class Subscription : DomainEntity
{
    public required string SubscriptionId { get; set; }
    public required Guid UserId { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset Expires { get; set; }
    public User User { get; set; } = default!;
}