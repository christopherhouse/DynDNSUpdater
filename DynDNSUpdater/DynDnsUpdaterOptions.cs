namespace DynDNSUpdater;

public class DynDnsUpdaterOptions
{
    public string? IpCheckHostName { get; set; }
    public string? IntervalToCheckInMinutes { get; set; }
    public string? DynDnsUpdateRequestUriFormat { get; set; }
    public string? HostNameToUpdate { get; set; }
    public string? DynDnsUsername { get; set; }
    public string? DynDnsPassword { get; set; }

    public int IntervalToCheckInMinutesAsMilliseconds
    {
        get
        {
            if (int.TryParse(IntervalToCheckInMinutes, out var interval))
            {
                return interval * 60 * 1000;
            }

            throw new ArgumentException("IntervalToCheckInMinutes must be a number");
        }
    }
}