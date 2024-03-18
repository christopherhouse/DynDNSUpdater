namespace DynDNSUpdater;

public class DynDnsUpdaterOptions
{
    public string? IpCheckHostName { get; set; }
    public int IntervalToCheckInMinutes { get; set; }
    public string? DynDnsUpdateRequestUriFormat { get; set; }
    public string? HostNameToUpdate { get; set; }
    public string? DynDnsUsername { get; set; }
    public string? DynDnsPassword { get; set; }
}