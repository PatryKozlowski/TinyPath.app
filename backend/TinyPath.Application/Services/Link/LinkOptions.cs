namespace TinyPath.Application.Services.Link;

public class LinkOptions
{
    public required string ChartSet { get; init; } = string.Empty;
    public required int ShortLinkLength { get; init; }
    public required int CustomShortLinkLength { get; init; }
    public required bool CustomShortLinkEnabled { get; init; }
    public required int GuestUserMaxLinkCount { get; init; }
    public required int GuestUserCreateLinkBlockTimeInMinutes { get; init; }
    public required string Host { get; init; } = string.Empty;
    public required List<string> ForbiddenKeywords { get; init; } = new();
    public required string HashidsLinkCodeSalt { get; init; }
}