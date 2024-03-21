namespace DynDNSUpdater.Services;

public interface IDnsUpdateHttpClient
{
    Task<string> GetCurrentIpAddressAsync();

    Task<bool> UpdateDynDns(string hostName, string ipAddress);
}