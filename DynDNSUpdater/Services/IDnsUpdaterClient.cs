namespace DynDNSUpdater.Services;

public interface IDnsUpdaterClient
{
    Task<DnsUpdaterClient.DynDnsStatsUpdateResult> UpdateDnsAsync(string hostName);
}