#nullable enable
using GoOS.GUI.Apps.GoWeb.Http;
using GoOS.GUI.Apps.GoWeb.Parsers;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class Document
    {
        private Document(Uri uri, string text, string contentType)
        {
            Uri = uri;
            Text = text;
            ContentType = contentType;
            if (contentType == "text/html")
            {
                Root = HtmlParser.Parse(text);
            }
            else
            {
                Root = new RootNode();
                if (!string.IsNullOrEmpty(text))
                {
                    TextNode textNode = new(text);
                    Root.Add(textNode);
                }
            }
            Body = (BodyElement)Root.QuerySelector("body")!;
            Title = Root.QuerySelector("title")?.TextContent.Trim() ?? uri.ToString();
        }

        private static Document LoadBuiltInPage(Uri uri)
        {
            switch (uri.Host)
            {
                case "blank":
                    return new Document(uri, string.Empty, "text/html");
                case "welcome":
                    return new Document(uri, @"<html>
<head>
    <title>Welcome</title>
</head>
<body>
    <h1>Welcome</h1>
    <hr>
    <p>GoWeb is the Web browser for GoOS.</p>
    <p>To get started, enter a location in the address bar.
<br>GoWeb is currently in alpha. Not all sites will work correctly.</p>
    <hr>
    <h2>Known Working URLs</h2>
    <pre>- http://example.org
- http://apps.goos.owen2k6.com
- http://mirrorservice.org
- http://mirrors.nav.ro</pre>
</body>
</html>", "text/html");
                default:
                    return new Document(uri, $"Unknown built-in page '{uri.Host}'.", "text/plain");
            }
        }

        private static Document LoadFromHttp(Uri uri)
        {
            HttpRequest request = new(uri);
            var response = request.Send();
            //response.Headers.TryGetValue("content-type", out string? contentType);
            return new Document(uri, response.Text, "text/html");
        }

        public static Document LoadFromUri(Uri uri)
        {
            switch (uri.Protocol)
            {
                case "about":
                    return LoadBuiltInPage(uri);
                case "http":
                    return LoadFromHttp(uri);
                default:
                    return new Document(uri, $"GoWeb does not know how to display the page '{uri.ToString()}'.", "text/plain");
            }
        }

        public Element Root { get; init; }

        public BodyElement Body { get; init; }

        public Uri Uri { get; init; }

        public string Text { get; init; }

        public string ContentType { get; init; }

        public string Title { get; set; }
    }
}
