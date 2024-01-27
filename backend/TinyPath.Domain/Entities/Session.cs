using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class Session : DomainEntity
{
    public required Guid UserId { get; set; }
    public required DateTimeOffset Expires { get; set; }
}