using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class EmailConfirmation : DomainEntity
{
    public required Guid Code { get; set; }
    public  Guid UserId { get; set; }
    public required DateTimeOffset Expires { get; set; }
    public bool Active { get; set; } = true;
    public User User { get; set; } = default!;
}