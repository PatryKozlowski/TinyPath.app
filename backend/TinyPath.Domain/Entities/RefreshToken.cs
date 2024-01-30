using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class RefreshToken : DomainEntity
{
    public required string Token { get; set; }
    public  Guid UserId { get; set; }
    public bool Active { get; set; } = true;
    public required DateTimeOffset Expires { get; set; }
    public User User { get; set; } = default!;
}