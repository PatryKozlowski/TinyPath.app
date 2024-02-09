using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class TemporaryLinkStats : DomainEntity
{
    public required Guid LinkId { get; set; }
    public int Visits { get; set; } = 0;
    public string Browser { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}