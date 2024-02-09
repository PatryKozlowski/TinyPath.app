using TinyPath.Domain.Common;
using TinyPath.Domain.Enums;

namespace TinyPath.Domain.Entities.TinyPath;

public class User : DomainEntity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
    public bool Blocked { get; set; } = false;
    public bool EmailConfirmed { get; set; } = false;
    public Session Session { get; set; } = default!;
    public RefreshToken RefreshToken { get; set; } = default!;
    public EmailConfirmation EmailConfirmation { get; set; } = default!;
    public Customer Customer { get; set; } = default!;
    public Subscription Subscription { get; set; } = default!;
    public ICollection<Link> Links { get; set; } = new List<Link>();
}