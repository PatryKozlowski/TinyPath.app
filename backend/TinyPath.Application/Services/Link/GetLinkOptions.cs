using Microsoft.Extensions.Options;

namespace TinyPath.Application.Services.Link;

public class GetLinkOptions : IGetLinkOptions
{
    private readonly LinkOptions _linkOptions;

    public GetLinkOptions(IOptions<LinkOptions> linkOptions)
    {
        _linkOptions = linkOptions.Value;
    }

    public int GetMaxLinkCountForGuestUser()
    {
        return _linkOptions.GuestUserMaxLinkCount;
    }

    public int GetBlockTimeInMinutesForGuestUser()
    {
        return _linkOptions.GuestUserCreateLinkBlockTimeInMinutes;
    }

    public int GetCustomShortLinkLength()
    {
        return _linkOptions.CustomShortLinkLength;
    }

    public string GetShortLinkHost()
    {
        return _linkOptions.Host;
    }

    public bool GetIsCustomShortLinkEnabled()
    {
        return _linkOptions.CustomShortLinkEnabled;
    }

    public List<string> GetForbiddenKeywords()
    {
        return _linkOptions.ForbiddenKeywords;
    }

    public string GetShortLinksChars()
    {
        return _linkOptions.ChartSet;
    }

    public int GetShortLinkLength()
    {
        return _linkOptions.ShortLinkLength;
    }

    public string GetHashidsLinkCodeSalt()
    {
        return _linkOptions.HashidsLinkCodeSalt;
    }
}