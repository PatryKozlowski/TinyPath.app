using TinyPath.Domain.Common;

namespace TinyPath.Domain.Entities.TinyPath;

public class Link : DomainEntity
{
    public Guid? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public bool Custom { get; set; } = false;
    public string? Title { get; set; }
    public required string Code { get; set; }
    public required string Url { get; set; }
    public required string OriginalUrl { get; set; }
    public bool Active { get; set; } = true;
    public User? User { get; set; }
    public Guest? Guest { get; set; }
    public LinkStats Stats { get; set; } = default!;
}