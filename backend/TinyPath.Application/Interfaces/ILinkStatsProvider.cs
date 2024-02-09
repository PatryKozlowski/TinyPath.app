namespace TinyPath.Application.Interfaces;

public interface ILinkStatsProvider
{
    string GetDeviceType();
    string GetBrowser();
    string GetOperatingSystem();
    Task<string?> GetCountry(string ipAddress);
}