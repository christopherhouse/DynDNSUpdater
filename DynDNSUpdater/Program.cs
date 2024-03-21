using DynDNSUpdater.Services;

namespace DynDNSUpdater;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        // Note since we're using env variables, the entire config has to be bound
        // to the options type
        builder.Services.AddOptions<DynDnsUpdaterOptions>()
            .Bind(builder.Configuration);

        // Register our services in the DI container
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddTransient<IDnsClient, DnsClient>();
        builder.Services.AddTransient<IDnsUpdaterClient, DnsUpdaterClient>();
        builder.Services.AddHttpClient<IDnsUpdateHttpClient, DnsUpdateHttpClient>();

        var host = builder.Build();
        host.Run();
    }
}