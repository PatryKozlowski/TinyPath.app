using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class LinkStat : DomainEntity
{
    public required Guid LinkId { get; set; }
    public int Visits { get; set; } = 0;
    public List<string> Browser { get; set; } = [];
    public List<string> Device { get; set; } = [];
    public List<string> Platform { get; set; } = [];
    public List<string> Country { get; set; } = [];
}