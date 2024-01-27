using TinyPath.Domain.Common;
using TinyPath.Domain.Enums;

namespace TinyPath.Domain.Entities;

public class User : DomainEntity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
    public bool Blocked { get; set; } = false;
    public bool EmailConfirmed { get; set; } = false;
}