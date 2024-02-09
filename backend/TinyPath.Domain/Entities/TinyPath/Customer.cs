using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class Customer : DomainEntity
{
    public required Guid UserId { get; set; }
    public required string CustomerId { get; set; }
    public required string Email { get; set; }
    public User User { get; set; } = default!;
}