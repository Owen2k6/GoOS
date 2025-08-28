#nullable enable
using System;
using System.Collections.Generic;

namespace GoOS.GUI.Apps.GoWeb.Http;

public class HttpResponse
{
    public HttpResponse(string message)
    {
        var parts = message.Split("\r\n\r\n", 2);
        if (parts.Length != 2) throw new Exception("Invalid HTTP response.");
        var lines = parts[0].Split("\r\n");
        Headers = new Dictionary<string, string>();
        for (var i = 1; i < lines.Length; i++)
        {
            var header = lines[i];
            var headerSplit = header.Split(": ");
            if (headerSplit.Length != 2) throw new Exception("Invalid HTTP header.\n" + header);
            Headers[headerSplit[0].ToLower()] = headerSplit[1];
        }

        Text = parts[1];
    }

    public string Text { get; init; }

    public Dictionary<string, string> Headers { get; init; }
}