using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Services.Link;

public class LinkManager : ILinkManager
{
    private readonly IGetLinkOptions _getLinkOptions;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<ILinkManager> _logger;
    private const string LOCALHOST = "localhost";

    public LinkManager(IGetLinkOptions getLinkOptions, IApplicationDbContext dbContext, ILogger<ILinkManager> logger)
    {
        _getLinkOptions = getLinkOptions;
        _dbContext = dbContext;
        _logger = logger;
    }

    public (string FullLink, string LinkCode) GenerateShortLink(string url, string? customCode = null, bool isCustom = false)
    {
        var isValidateUrl = ValidateUrl(url);
        
        if (!isValidateUrl)
        {
            throw new ErrorException("InvalidUrl");
        }
        
        var shortLinkHost = _getLinkOptions.GetShortLinkHost();
        
        string fullLink;
        string linkCode;
        
        if (isCustom && customCode is not null)
        {
            var isValidCode = ValidateCustomCode(customCode);
            
            if (isValidCode)
            {
                throw new ErrorException("InvalidCustomCode");
            }
            
            fullLink = $"{shortLinkHost}/{customCode}";
            linkCode = customCode;
        }
        else
        {
            var shortLinkCode = GenerateShortLinkCode();
            
            fullLink = $"{shortLinkHost}/{shortLinkCode}";
            linkCode = shortLinkCode;
        }
        
        return (fullLink, linkCode);
    }
    
    private bool ValidateUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
               && !string.IsNullOrEmpty(url) && !url.Contains(LOCALHOST);
    }
    
    private bool ValidateCustomCode(string code)
    {
        var forbiddenWords = _getLinkOptions.GetForbiddenKeywords();

        return forbiddenWords.Any(word => code.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
    
    private string GenerateShortLinkCode()
    {
        var chars = _getLinkOptions.GetShortLinksChars();
        var shortLinkLength = _getLinkOptions.GetShortLinkLength();
        var hashidsSalt = _getLinkOptions.GetHashidsLinkCodeSalt();
        
        var hashids = new Hashids(hashidsSalt, shortLinkLength, chars);
        
        var random = new Random();
        
        var minValue = (int)Math.Pow(10, shortLinkLength - 1);
        var maxValue = (int)Math.Pow(10, shortLinkLength) - 1;
        
        var shortLinkCode = random.Next(minValue, maxValue);

        return hashids.Encode(shortLinkCode);
    }

    public async Task AggregateLinkStats()
    {
        _logger.LogInformation("Aggregated link stats start");
        
        var aggregatedStats =  await _dbContext.TemporaryLinkStats
            .GroupBy(temporaryStats => temporaryStats.LinkId)
            .Select(group => new LinkStats
            {
                LinkId = group.Key,
                Visits = group.Sum(temporaryStats => temporaryStats.Visits),
                Browser = group.Select(temporaryStats => temporaryStats.Browser).ToList(),
                Device = group.Select(temporaryStats => temporaryStats.Device).ToList(),
                Platform = group.Select(temporaryStats => temporaryStats.Platform).ToList(),
                Country = group.Select(temporaryStats => temporaryStats.Country).ToList()
            })
            .ToListAsync();
        
        _logger.LogInformation($"Aggregated stats count: {aggregatedStats.Count}");
        
        if (aggregatedStats.Count == 0)
        {
            return;
        }

        foreach (var aggregatedStat in aggregatedStats)
        {
            var existingLinkStat = await _dbContext.LinksStats
                .FirstOrDefaultAsync(ls => ls.LinkId == aggregatedStat.LinkId);

            if (existingLinkStat != null)
            {
                existingLinkStat.Visits += aggregatedStat.Visits;
                existingLinkStat.Browser.AddRange(aggregatedStat.Browser);
                existingLinkStat.Device.AddRange(aggregatedStat.Device);
                existingLinkStat.Platform.AddRange(aggregatedStat.Platform);
                existingLinkStat.Country.AddRange(aggregatedStat.Country);
            }
            else
            {
                _dbContext.LinksStats.AddRange(aggregatedStat);
            }
        }
        
        
        var linkIdsToDelete = aggregatedStats.Select(stats => stats.LinkId).ToList();
        var temporaryLinkStatsToDelete = await _dbContext.TemporaryLinkStats
            .Where(tempStats => linkIdsToDelete.Contains(tempStats.LinkId))
            .ToListAsync();
        
        
        _dbContext.TemporaryLinkStats.RemoveRange(temporaryLinkStatsToDelete);
        
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Aggregated link stats finished");
    }

    public async Task<(int totalVisits, string? linkUrl)> GetLasGuestLinkVisitsByLinkId(Guid linkId)
    {
        var linkUrl = await _dbContext.Links
            .Where(x => x.Id == linkId)
            .Select(x => x.Url)
            .FirstOrDefaultAsync();
        
        if (linkUrl is null)
        {
            throw new ErrorException("LinkNotFound");
        }
        
        var getLinksVisitsFromTemporaryStats = await _dbContext.TemporaryLinkStats
            .Where(x => x.LinkId == linkId)
            .SumAsync(x => x.Visits);
        
        var getLinksVisitsFromStats = await _dbContext.LinksStats
            .Where(x => x.LinkId == linkId)
            .Select(x => x.Visits)
            .FirstOrDefaultAsync();
        
        var totalVisits = getLinksVisitsFromTemporaryStats + getLinksVisitsFromStats;
        
        return (totalVisits, linkUrl);
    }
    
    public async Task<Guid> GetLastGuestUserLinkId(Guid guestId)
    {
        var lastGuestUserLink = await _dbContext.Links
            .Where(x => x.GuestId == guestId)
            .OrderByDescending(x => x.Created)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        
        return lastGuestUserLink;
    }
}