using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class Session : DomainEntity
{
    public Guid UserId { get; set; }
    public required DateTimeOffset Expires { get; set; }
    public string HangfireId { get; set; } = string.Empty;
    public User User { get; set; } = default!;
}