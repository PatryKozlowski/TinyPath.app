namespace TinyPath.Application.Services.Link;

public interface ILinkManager
{
    (string FullLink, string LinkCode) GenerateShortLink(string url, string? customCode = null, bool isCustom = false);
    Task AggregateLinkStats();
}