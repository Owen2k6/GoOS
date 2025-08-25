using System;
using System.IO;
using GoOS.Tasking;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS;
using Gold.Graphics;
using Gold.Graphics.Fonts;
using GoOS.Commands;
using GoOS.Themes;
using LibDotNetParser.CILApi;
using System;
using static GoOS.Kernel;

namespace GoOS.GUI.Apps.Terminal;

public abstract class Shell
{
    internal SVGAIITerminal _terminal;

    internal Shell(SVGAIITerminal Terminal)
    {
        _terminal = Terminal;
    }

    internal abstract void Run();

    public void HandleGoCommand(string[] args)
    {
        string sub = args[1];

        if (sub == "type")
        {
            log(Color.Minty, "GoOS - Application Types");
            log(Color.GoogleYellow, "-g Goexe");
            log(Color.GoogleYellow, "-9 9xCode");
            return;
        }

        if (sub == "install")
        {
            if (args.Length != 5)
            {
                log(ThemeManager.ErrorText, "X: go install <repo> <appname> -<type>");
                return;
            }

            DownloadApplication(args[2], args[3], args[4]);
            return;
        }

        log(ThemeManager.ErrorText, "Unknown request.");
    }

    public void NavigateToParentDirectory()
    {
        try
        {
            string currentDir = Directory.GetCurrentDirectory().TrimEnd('\\');
            int lastSlashIndex = currentDir.LastIndexOf('\\');

            if (lastSlashIndex > 2) // keep "0:\"
                Directory.SetCurrentDirectory(currentDir.Remove(lastSlashIndex));
            else
                Directory.SetCurrentDirectory(@"0:\");
        }
        catch
        {
            Directory.SetCurrentDirectory(@"0:\");
        }
    }

    public bool CheckRamAndArguments(int totalRam, string[] args, int expectedCount)
    {
        if (totalRam < 1000)
        {
            log(ThemeManager.ErrorText, "This programme has been disabled due to low RAM.");
            return false;
        }

        return CheckArgCount(args, expectedCount, true);
    }

    public bool CheckArgCount(string[] args, int expectedCount, bool exact)
    {
        if (args.Length < expectedCount)
        {
            log(ThemeManager.ErrorText, "Missing arguments");
            return false;
        }

        if (exact && args.Length > expectedCount)
        {
            log(ThemeManager.ErrorText, "Too many arguments");
            return false;
        }

        return true;
    }

    public void log(Color colour, string str)
    {
        _terminal.ForegroundColor = colour;
        _terminal.WriteLine(str);
    }

    public void DownloadApplication(string repo, string fileToGet, string typeFlag)
    {
        try
        {
            string type;
            if (typeFlag == "-g") type = "goexe";
            else if (typeFlag == "-9") type = "9xc";
            else
            {
                log(ThemeManager.ErrorText, "Unknown application type");
                return;
            }

            log(Color.Red, "Downloading " + fileToGet + "." + type + " from " + repo);

            // DNS resolve
            var dnsClient = new DnsClient();
            dnsClient.Connect(DNSConfig.DNSNameservers[0]);
            dnsClient.SendAsk(repo);
            Address address = dnsClient.Receive();
            dnsClient.Close();

            // TCP GET
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(address.ToString(), 80);

                using (NetworkStream stream = tcpClient.GetStream())
                {
                    string request =
                        "GET /" + fileToGet + "." + type + " HTTP/1.1\r\n" +
                        "User-Agent: GoOS\r\n" +
                        "Accept: */*\r\n" +
                        "Accept-Encoding: identity\r\n" +
                        "Host: " + repo + "\r\n" +
                        "Connection: close\r\n\r\n";

                    byte[] dataToSend = Encoding.ASCII.GetBytes(request);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    string filePath = @"0:\" + fileToGet + "." + type;

                    byte[] buffer = new byte[4096];
                    int headerEndAt = -1;
                    int bytesRead;
                    int headerLen = 0;
                    byte[] headerBuf = new byte[8192];

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (headerEndAt < 0)
                        {
                            int toCopy = bytesRead;
                            if (headerLen + toCopy > headerBuf.Length)
                                toCopy = headerBuf.Length - headerLen;

                            for (int i = 0; i < toCopy; i++)
                                headerBuf[headerLen + i] = buffer[i];
                            headerLen += toCopy;

                            headerEndAt = FindHeaderEnd(headerBuf, headerLen);
                            if (headerEndAt >= 0)
                            {
                                int firstLineEnd = IndexOfCrlf(headerBuf, 0, headerEndAt + 4);
                                string statusLine = GetAscii(headerBuf, 0, firstLineEnd);
                                if (statusLine.IndexOf(" 200") < 0)
                                {
                                    Dialogue.Show("GoOS Update", "HTTP error: " + statusLine, default,
                                        Resources.errorIcon);
                                    return;
                                }

                                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                {
                                    // body bytes already in headerBuf
                                    int bodyFromHeaderBuf = headerLen - (headerEndAt + 4);
                                    if (bodyFromHeaderBuf > 0)
                                        fs.Write(headerBuf, headerEndAt + 4, bodyFromHeaderBuf);

                                    // any unread portion of current buffer (not copied into headerBuf)
                                    int remainingFromCurrentRead = bytesRead - toCopy;
                                    if (remainingFromCurrentRead > 0)
                                        fs.Write(buffer, toCopy, remainingFromCurrentRead);

                                    // stream remaining body
                                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                        fs.Write(buffer, 0, bytesRead);
                                }

                                log(Color.Green, "Downloaded " + fileToGet + "." + type);
                                return;
                            }

                            if (headerLen == headerBuf.Length)
                            {
                                Dialogue.Show("GoOS Update", "Invalid HTTP response (headers too large).", default,
                                    Resources.errorIcon);
                                return;
                            }

                            continue;
                        }
                    }

                    Dialogue.Show("GoOS Update", "Invalid or empty HTTP response!", default,
                        Resources.errorIcon);
                }
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
    }

    private static int FindHeaderEnd(byte[] buf, int len)
    {
        for (int i = 0; i <= len - 4; i++)
        {
            if (buf[i] == 13 && buf[i + 1] == 10 && buf[i + 2] == 13 && buf[i + 3] == 10)
                return i;
        }

        return -1;
    }

    private static int IndexOfCrlf(byte[] buf, int start, int max)
    {
        for (int i = start; i < max - 1; i++)
            if (buf[i] == 13 && buf[i + 1] == 10)
                return i;
        return -1;
    }

    private static string GetAscii(byte[] buf, int start, int endExclusive)
    {
        int n = endExclusive - start;
        if (n <= 0) return "";
        return Encoding.ASCII.GetString(buf, start, n);
    }

    public static bool EndsWithIgnoreCase(string text, string suffix)
    {
        int tl = text.Length, sl = suffix.Length;
        if (sl > tl) return false;
        int off = tl - sl;
        for (int i = 0; i < sl; i++)
        {
            char a = text[off + i];
            char b = suffix[i];
            if (a >= 'A' && a <= 'Z') a = (char)(a + 32);
            if (b >= 'A' && b <= 'Z') b = (char)(b + 32);
            if (a != b) return false;
        }

        return true;
    }
}