using System.Net;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TinyPath.Application.Interfaces;

namespace TinyPath.WebApi.Application.Link;

public class LinkStatsProvider : ILinkStatsProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGeoIpDbContext _geoIpDbContext;

    public LinkStatsProvider(IHttpContextAccessor httpContextAccessor, IGeoIpDbContext geoIpDbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _geoIpDbContext = geoIpDbContext;
    }

    private const string USER_AGENT = "User-Agent";
    
    private string UserAgent() => _httpContextAccessor.HttpContext?.Request.Headers[USER_AGENT].ToString()!;
    
    private string MatchUserAgent(string userAgent, params (string Substring, string Value)[] matches)
    {
        foreach (var (substring, value) in matches)
        {
            if (userAgent.Contains(substring, StringComparison.CurrentCultureIgnoreCase))
            {
                return value;
            }
        }

        return "Unknown";
    }
    
    public string GetDeviceType()
    {
        var userAgent = UserAgent();
        
        if (userAgent.Contains("Windows", StringComparison.CurrentCultureIgnoreCase) 
            || userAgent.Contains("Macintosh", StringComparison.CurrentCultureIgnoreCase) 
            || userAgent.Contains("Linux"))
        {
            return "Desktop";
        }

        return MatchUserAgent(userAgent,
            ("mobile", "Mobile"),
            ("Iphone", "Mobile"),
            ("tablet", "Tablet"));
    }

    public string GetBrowser()
    {
        var userAgent = UserAgent();
        
        return MatchUserAgent(userAgent,
            ("edge", "Edge"),
            ("edg", "Edge"),
            ("chrome", "Chrome"),
            ("safari", "Safari"),
            ("firefox", "Firefox"),
            ("opera", "Opera"),
            ("msie", "Internet Explorer"));

    }

    public string GetOperatingSystem()
    {
        var userAgent = UserAgent();
        
        return MatchUserAgent(userAgent,
            ("windows", "Windows"),
            ("macintosh", "MacOs"),
            ("linux", "Linux"));
    }

    public async Task<string?> GetCountry(string ipAddress)
    {
        var query = @"SELECT ""CountryCode"" 
              FROM ""Country""
              WHERE @Address::inet BETWEEN ""IpStart"" AND ""IpEnd"" 
              LIMIT 1";

        var country = await _geoIpDbContext.Country
            .FromSqlRaw(query, new NpgsqlParameter("Address", ipAddress))
            .Select(c => c.CountryCode)
            .FirstOrDefaultAsync();
        
        return country;
    }
}