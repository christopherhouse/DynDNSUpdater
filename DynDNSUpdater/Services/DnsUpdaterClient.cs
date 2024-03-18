using Microsoft.Extensions.Options;

namespace DynDNSUpdater.Services;

public class DnsUpdaterClient : IDnsUpdaterClient
{
    private readonly IOptions<DynDnsUpdaterOptions> _options;
    private readonly IDnsClient _dnsClient;
    private readonly IDnsUpdateHttpClient _dnsUpdateHttpClient;
    private readonly ILogger<DnsUpdaterClient> _logger;


    public DnsUpdaterClient(IOptions<DynDnsUpdaterOptions> options,
        IDnsClient dnsClient,
        IDnsUpdateHttpClient dnsUpdateHttpClient,
        ILogger<DnsUpdaterClient> logger)
    {
        _options = options;
        _dnsClient = dnsClient;
        _dnsUpdateHttpClient = dnsUpdateHttpClient;
        _logger = logger;
    }

    public async Task<DynDnsStatsUpdateResult> UpdateDnsAsync(string hostName)
    {
        DynDnsStatsUpdateResult result;

        var currentDnsIp = await _dnsClient.GetIpForHostNameAsync(hostName);
        var actualCurrentIp = await _dnsUpdateHttpClient.GetCurrentIpAddressAsync();

        if (currentDnsIp == actualCurrentIp)
        {
            _logger.LogInformation($"No update needed, current IP in DNS is: {currentDnsIp}");
            result = DynDnsStatsUpdateResult.NoUpdateNeeded;
        }
        else
        {
            var updateResult= await _dnsUpdateHttpClient.UpdateDynDns(hostName, actualCurrentIp);

            if (updateResult)
            {
                _logger.LogInformation($"Update succeeded, new IP in DNS is: {actualCurrentIp}");
                result = DynDnsStatsUpdateResult.UpdateSucceeded;
            }
            else
            {
                _logger.LogError($"Update failed, current IP in DNS is: {currentDnsIp}");
                result = DynDnsStatsUpdateResult.UpdateFailed;
            }
        }

        _logger.LogInformation($"Current IP in DNS is: {currentDnsIp}");

        return result;
    }

    public enum DynDnsStatsUpdateResult
    {
        UpdateSucceeded,
        UpdateFailed,
        NoUpdateNeeded
    }
}