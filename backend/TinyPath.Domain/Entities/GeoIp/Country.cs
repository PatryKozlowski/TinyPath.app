using System.Net;

namespace TinyPath.Domain.Entities.GeoIp;

public class Country
{
    public string IpStart { get; set; } = null!;
    public string IpEnd { get; set; } = null!;
    public string CountryCode { get; set; } = string.Empty;
}