using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System.Collections.Generic;

namespace GoOS.Networking
{
    public static class FastDns
    {
        private static Dictionary<string, Address> _cache = new();

        public static Address Resolve(string host)
        {
            if (_cache.ContainsKey(host))
            {
                return _cache[host];
            }
            var dnsClient = new DnsClient();
            dnsClient.Connect(DNSConfig.DNSNameservers[0]);
            dnsClient.SendAsk(host);
            Address address = dnsClient.Receive();
            dnsClient.Close();
            _cache.Add(host, address);
            return address;
        }
    }
}
