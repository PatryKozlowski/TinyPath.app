using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Services.Redirect;

public class RedirectManager : IRedirectManager
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILinkStatsProvider _linkStatsProvider;
    private readonly IAuthDataProvider _authDataProvider;

    public RedirectManager(IApplicationDbContext dbContext, ILinkStatsProvider linkStatsProvider, IAuthDataProvider authDataProvider)
    {
        _dbContext = dbContext;
        _linkStatsProvider = linkStatsProvider;
        _authDataProvider = authDataProvider;
    }

    public async Task<string> HandleRedirectLink(string linkCode)
    {
        var link = await _dbContext.Links
            .Where(x => x.Code == linkCode)
            .Where(x => x.Active)
            .FirstOrDefaultAsync();

        if (link == null)
        {
            throw new ErrorException("LinkNotFound");
        }
        
        return link.OriginalUrl;
    }

    public async Task UpdateLinkStatsAfterRedirect(Guid linkId)
    {
        var ipAddress = _authDataProvider.GetRemoteIpAddress();
        
        var device = _linkStatsProvider.GetDeviceType();
        var browser = _linkStatsProvider.GetBrowser();
        var os = _linkStatsProvider.GetOperatingSystem();
        var country = await _linkStatsProvider.GetCountry(ipAddress);
        
        var linkStats = new TemporaryLinkStats
        {
            LinkId = linkId,
            Visits = 1,
            Device = device,
            Browser = browser,
            Platform = os,
            Country = country!,
        };
        
        _dbContext.TemporaryLinkStats.Add(linkStats);
        
        await _dbContext.SaveChangesAsync();
    }
}