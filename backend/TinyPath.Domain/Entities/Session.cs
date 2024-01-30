using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class Session : DomainEntity
{
    public Guid UserId { get; set; }
    public required DateTimeOffset Expires { get; set; }
    public User User { get; set; } = default!;
}