namespace TinyPath.Application.Services.Redirect;

public interface IRedirectManager
{
    Task<string> HandleRedirectLink(string linkCode);
    Task UpdateLinkStatsAfterRedirect(Guid linkId);
}