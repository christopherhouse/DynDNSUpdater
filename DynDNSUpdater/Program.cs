using DynDNSUpdater.Services;

namespace DynDNSUpdater;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddOptions<DynDnsUpdaterOptions>()
            .Bind(builder.Configuration);

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddTransient<IDnsClient, DnsClient>();
        builder.Services.AddTransient<IDnsUpdaterClient, DnsUpdaterClient>();
        builder.Services.AddHttpClient<IDnsUpdateHttpClient, DnsUpdateHttpClient>();

        var host = builder.Build();
        host.Run();
    }
}