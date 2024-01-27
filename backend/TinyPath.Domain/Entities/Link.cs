using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities;

public class Link : DomainEntity
{
    public Guid? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public bool Custom { get; set; } = false;
    public required string Code { get; set; }
    public required string Url { get; set; }
    public required string OriginalUrl { get; set; }
    public bool Active { get; set; } = true;
}