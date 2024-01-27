using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class Guest : DomainEntity
{
    public required string IpAddress { get; set; }
    public int Links { get; set; } = 0;
    public bool Blocked { get; set; } = false;
    public DateTimeOffset? BlockedUntil { get; set; }
}