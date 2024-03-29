using DynDNSUpdater.Services;
using Microsoft.Extensions.Options;

namespace DynDNSUpdater;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DynDnsUpdaterOptions _options;
    private readonly IDnsUpdaterClient _dnsUpdaterClient;
    private bool _isFirstRun = true;
    private readonly string _hostNameToUpdate;

    public Worker(IOptions<DynDnsUpdaterOptions> options,
        IDnsUpdaterClient dnsUpdaterClient,
        ILogger<Worker> logger)
    {
        _logger = logger;
        _options = options.Value;
        _dnsUpdaterClient = dnsUpdaterClient;

        if (string.IsNullOrWhiteSpace(_options.HostNameToUpdate))
        {
            throw new ArgumentException("HostNameToUpdate must be set");
        }

        _hostNameToUpdate = _options.HostNameToUpdate;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Initializing worker, polling interval is {_options.IntervalToCheckInMinutes} minutes/{_options.IntervalToCheckInMinutesAsMilliseconds} ms");


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var initialLogMessage = _isFirstRun ? "Initial run - updating DNS" : "Delay interval elapsed - updating DNS";
                _logger.LogInformation(initialLogMessage);

                var result = await _dnsUpdaterClient.UpdateDnsAsync(_hostNameToUpdate);
                _isFirstRun = false;

                switch (result)
                {
                    case DnsUpdaterClient.DynDnsStatsUpdateResult.NoUpdateNeeded:
                        _logger.LogInformation($"No update needed");
                        break;
                    case DnsUpdaterClient.DynDnsStatsUpdateResult.UpdateSucceeded:
                        _logger.LogInformation($"Update succeeded");
                        break;
                    case DnsUpdaterClient.DynDnsStatsUpdateResult.UpdateFailed:
                        _logger.LogError($"Update failed, review logs for exceptions");
                        break;
                    default:
                        // Should never get here, but let's make R# happy, I guess :D
                        throw new ArgumentOutOfRangeException();
                }

                _logger.LogInformation($"DNS update complete with result: {result}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating DNS");
            }

            await Task.Delay(_options.IntervalToCheckInMinutesAsMilliseconds, stoppingToken);
        }
    }
}
