using System;

namespace GoOS.GUI.Apps.GoWeb.Http
{
    public class Uri
    {
        public Uri(string protocol, string host, string path)
        {
            Protocol = protocol;
            Host = host;
            Path = path;
        }

        /*
         * TODOTODO: Decode escaped characters.
         */
        public static Uri FromString(string uri)
        {
            uri = uri.Trim();
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Invalid URI");
            }

            int protoIdx = uri.IndexOf(':');
            string proto;
            if (protoIdx != -1)
            {
                proto = uri.Substring(0, protoIdx);
            }
            else
            {
                throw new ArgumentException("Invalid URI");
            }

            int pathIdx = uri.IndexOf('/', protoIdx + (uri.StartsWith(proto + "://") ? 3 : 1));

            string path = pathIdx != -1 ? uri.Substring(pathIdx) : string.Empty;

            int queryIdx = path.IndexOf('?');
            if (queryIdx != -1)
                path = path.Substring(0, queryIdx); // drop query
            int fgmtIdx = path.IndexOf('#');
            if (fgmtIdx != -1)
                path = path.Substring(0, fgmtIdx); // drop fgmt

            string host = uri.Substring(protoIdx + 1, uri.Length - path.Length - protoIdx - 1);

            return new Uri(proto, host, path);
        }

        public override string ToString()
        {
            return $"{Protocol}:{Host}{Path}";
        }

        public string Protocol { get; init; }

        public string Host { get; init; }

        public string Path { get; init; }
    }
}
