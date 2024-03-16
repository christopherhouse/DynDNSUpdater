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

        foreach (var c in builder.Configuration.AsEnumerable())
        {
            Console.WriteLine($"** {c.Key}: {c.Value}");
        }

        var host = builder.Build();
        host.Run();
    }
}