// HttpHelper.cs

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;

namespace GoOS.Networking;

public static class HttpHelper
{
    // -------------- Configuration --------------
    // Markers that identify your “country not available” HTML, in case the edge returns 200.
    private static readonly string[] BlockBodyMarkers =
    {
        "<title>Owen2k6 Network is not available in your country",
        "You are connecting from a country where 2k6 Network is not available.",
        "United Kingdom Notice:",
        "Online Safety Act 2023",
        "Hinweis für Deutschland"
    };

    // -------------- Public API --------------
    public static string SimpleHttpGet(string host, string path)
    {
        var resp = SimpleHttpGetRaw(host, path);
        CheckForRegionalBlock(resp);
        return resp.Body;
    }

    public static HttpResponse SimpleHttpGetRaw(string host, string path)
    {
        var serverIP = ResolveDNS(host);
        if (string.IsNullOrEmpty(serverIP))
            throw new Exception("DNS resolution failed");

        using (var tcpClient = new TcpClient())
        {
            tcpClient.Connect(serverIP, 80);

            using (var stream = tcpClient.GetStream())
            {
                var httpget =
                    "GET " + path + " HTTP/1.1\r\n" +
                    "User-Agent: GoOS\r\n" +
                    "Accept: */*\r\n" +
                    "Accept-Encoding: identity\r\n" + // avoid compression
                    "Host: " + host + "\r\n" +
                    "Connection: close\r\n\r\n";

                var req = Encoding.ASCII.GetBytes(httpget);
                stream.Write(req, 0, req.Length);

                // Read-all (Connection: close)
                var buf = new byte[8192];
                int read;
                var sb = new StringBuilder(8192);
                do
                {
                    read = stream.Read(buf, 0, buf.Length);
                    if (read > 0) sb.Append(Encoding.ASCII.GetString(buf, 0, read));
                } while (read > 0);

                var raw = sb.ToString();
                var headerEnd = raw.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                if (headerEnd < 0) throw new Exception("Invalid HTTP response");

                var headerSection = raw.Substring(0, headerEnd);
                var body = raw.Substring(headerEnd + 4);

                var firstCr = headerSection.IndexOf("\r\n", StringComparison.Ordinal);
                if (firstCr < 0) throw new Exception("Invalid HTTP response (no status line)");
                var statusLine = headerSection.Substring(0, firstCr);
                var parts = statusLine.Split(' ');
                var code = parts.Length >= 2 && int.TryParse(parts[1], out var c) ? c : 0;
                var reason = parts.Length >= 3 ? string.Join(" ", parts, 2, parts.Length - 2) : "";

                var resp = new HttpResponse { StatusCode = code, ReasonPhrase = reason, Body = body };

                var lines = headerSection.Substring(firstCr + 2).Split(new[] { "\r\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    var colon = line.IndexOf(':');
                    if (colon > 0)
                    {
                        var name = line.Substring(0, colon).Trim();
                        var value = line.Substring(colon + 1).Trim();
                        if (!resp.Headers.ContainsKey(name))
                            resp.Headers.Add(name, value);
                    }
                }

                // Basic dechunk if edge insists (we requested identity)
                if (resp.Headers.TryGetValue("Transfer-Encoding", out var te) &&
                    te.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0)
                    resp.Body = Dechunk(resp.Body);

                return resp;
            }
        }
    }

    public static void CheckForRegionalBlock(HttpResponse resp)
    {
        if (resp.StatusCode == 401)
            throw new RegionBlockedException("You are connecting from a country where 2k6 Network is not available.");
    }

    // -------------- Internals --------------
    private static string ResolveDNS(string host)
    {
        var dnsClient = new DnsClient();
        dnsClient.Connect(DNSConfig.DNSNameservers[0]);
        dnsClient.SendAsk(host);
        var address = dnsClient.Receive();
        dnsClient.Close();
        return address.ToString();
    }

    private static string Dechunk(string body)
    {
        var pos = 0;
        var sb = new StringBuilder(body.Length);
        while (true)
        {
            var lineEnd = body.IndexOf("\r\n", pos, StringComparison.Ordinal);
            if (lineEnd < 0) break;
            var sizeLine = body.Substring(pos, lineEnd - pos).Trim();
            if (!int.TryParse(sizeLine, NumberStyles.HexNumber, null, out var size)) break;
            pos = lineEnd + 2;
            if (size == 0) break;
            if (pos + size > body.Length) break;
            sb.Append(body.Substring(pos, size));
            pos += size;
            if (pos + 2 <= body.Length) pos += 2; // CRLF
        }

        return sb.ToString();
    }

    // -------------- Types --------------
    public sealed class HttpResponse
    {
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }

        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

        public string Body { get; set; } = string.Empty;
    }

    public sealed class RegionBlockedException : Exception
    {
        public RegionBlockedException(string message) : base(message)
        {
        }
    }
}