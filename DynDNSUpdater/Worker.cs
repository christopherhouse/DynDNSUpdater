using DynDNSUpdater.Services;
using Microsoft.Extensions.Options;

namespace DynDNSUpdater;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<DynDnsUpdaterOptions> _options;
    private readonly IDnsUpdaterClient _dnsUpdaterClient;
    private readonly int delayIntervalInMilliseconds;

    public Worker(IOptions<DynDnsUpdaterOptions> options,
        IDnsUpdaterClient dnsUpdaterClient,
        ILogger<Worker> logger)
    {
        _logger = logger;
        _options = options;
        _dnsUpdaterClient = dnsUpdaterClient;
        delayIntervalInMilliseconds = options.Value.IntervalToCheckInMinutes * 60 * 1000;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"** Initializing worker, polling interval is {_options.Value.IntervalToCheckInMinutes} minutes/{delayIntervalInMilliseconds} ms");


        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                Console.WriteLine($"** {_options.Value.HostNameToUpdate}");

                var result = await _dnsUpdaterClient.UpdateDnsAsync(_options.Value.HostNameToUpdate);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(delayIntervalInMilliseconds, stoppingToken);
        }
    }
}
