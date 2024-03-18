using System.Net;

namespace DynDNSUpdater.Services;

public  class DnsClient : IDnsClient
{
    public async Task<string> GetIpForHostNameAsync(string hostName)
    {
        // Return the IP address for the hostName specified by the
        // hostName parameter.
        var addresses = await Dns.GetHostAddressesAsync(hostName);

        if (addresses.Length == 0)
        {
            throw new InvalidOperationException("No IP addresses found for the host name");
        }

        return addresses[0].ToString();
    }
}