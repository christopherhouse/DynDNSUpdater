﻿using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace DynDNSUpdater.Services;

public class DnsUpdateHttpClient : IDnsUpdateHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly DynDnsUpdaterOptions _options;
    private readonly Uri _checkCurrentIpUri;
    private string _dynDnsUpdateRequestUriFormat;

    public DnsUpdateHttpClient(HttpClient _httpClient, IOptions<DynDnsUpdaterOptions> options)
    {
        this._httpClient = _httpClient;
        _options = options.Value;
        _checkCurrentIpUri = new Uri($"http://{_options.IpCheckHostName}"); // Host only supports HTTP

        if (string.IsNullOrWhiteSpace(_options.DynDnsUpdateRequestUriFormat))
        {
            throw new ArgumentException("DynDnsUpdateRequestUriFormat must be set");
        }

        _dynDnsUpdateRequestUriFormat = _options.DynDnsUpdateRequestUriFormat;
    }

    public async Task<string> GetCurrentIpAddressAsync()
    {
        var response = await _httpClient.GetAsync(_checkCurrentIpUri);

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        // Format of string is:
        // <html><head><title>Current IP Check</title></head><body>Current IP Address: 65.128.35.27</body></html>
        // Using a regex, extract the IP address from the responseString.  The ip address will always be in the same
        // location, after the "Current IP Address: " string.  The regex should only extract the value of the IP address.
        var regex = new Regex(@"Current IP Address: (\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");

        var currentIp = regex.Match(responseString).Groups[1].Value;

        return currentIp;
    }

    public async Task<bool> UpdateDynDns(string hostName, string ipAddress)
    {
        var reqUri = FormatUpdateUri(hostName, ipAddress);
        var response = await _httpClient.GetAsync(reqUri);
        response.EnsureSuccessStatusCode();

        return true;
    }

    private Uri FormatUpdateUri(string hostName, string ipAddress)
    {
        var authToken = $"{_options.DynDnsUsername}:{_options.DynDnsPassword}";
        var uriString = string.Format(_dynDnsUpdateRequestUriFormat, authToken, hostName, ipAddress);

        return new Uri(uriString);
    }
}