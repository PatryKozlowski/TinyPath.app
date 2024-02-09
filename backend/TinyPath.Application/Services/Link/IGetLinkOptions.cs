namespace TinyPath.Application.Services.Link;

public interface IGetLinkOptions
{
    int GetMaxLinkCountForGuestUser();
    int GetBlockTimeInMinutesForGuestUser();
    int GetCustomShortLinkLength();
    string GetShortLinkHost();
    bool GetIsCustomShortLinkEnabled();
    List<string> GetForbiddenKeywords();
    string GetShortLinksChars();
    int GetShortLinkLength();
    string GetHashidsLinkCodeSalt();
}