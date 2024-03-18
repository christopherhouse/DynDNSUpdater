namespace DynDNSUpdater.Services;

public interface IDnsClient
{
    Task<string> GetIpForHostNameAsync(string hostName);
}