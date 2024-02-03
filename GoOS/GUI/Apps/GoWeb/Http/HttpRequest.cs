#nullable enable
using Cosmos.System.Network.IPv4;
using System.Net.Sockets;
using System.Text;
using System;
using System.Collections.Generic;
using GoOS.Networking;

namespace GoOS.GUI.Apps.GoWeb.Http
{
    public class HttpRequest
    {
        public HttpRequest(Uri uri)
        {
            _uri = uri;

            if (!_uri.Host.StartsWith("//"))
                throw new InvalidOperationException("Invalid host.");
            _unrootedHost = _uri.Host.Substring(2);
            if (string.IsNullOrEmpty(_unrootedHost))
                throw new InvalidOperationException("Invalid host.");

            Headers = new()
            {
                ["User-Agent"] = $"Mozilla/4.0 (GoOS {Kernel.version}; like MSIE 1.0) GoWeb/1.0",
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
                ["Accept-Language"] = "en-GB,en;q=0.9",
                ["Accept-Encoding"] = "identity",
                ["Host"] = _unrootedHost,
                ["Connection"] = "close"
            };
        }

        public HttpResponse Send()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                Address address = FastDns.Resolve(_unrootedHost);

                tcpClient.Connect(address.ToString(), 80);
                NetworkStream stream = tcpClient.GetStream();

                StringBuilder requestBuilder = new();
                requestBuilder.Append("GET ");
                requestBuilder.Append(_uri.Path.Length > 0 ? _uri.Path : "/");
                requestBuilder.Append(" HTTP/1.1\r\n");
                foreach (KeyValuePair<string, string> header in Headers)
                {
                    requestBuilder.Append(header.Key);
                    requestBuilder.Append(": ");
                    requestBuilder.Append(header.Value);
                    requestBuilder.Append("\r\n");
                }
                requestBuilder.Append("\r\n");

                byte[] dataToSend = Encoding.ASCII.GetBytes(requestBuilder.ToString());
                stream.Write(dataToSend, 0, dataToSend.Length);

                /*
                 * TODOTODO: receive responses longer than 8K
                 * TODOTODO: receive binary responses
                 * 
                 * REF: <github.com/CosmosOS/CosmosHttp>
                 */

                byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                return new HttpResponse(receivedMessage);
            }
        }

        public Dictionary<string, string> Headers { get; init; }

        readonly Uri _uri;

        readonly string _unrootedHost;
    }
}
