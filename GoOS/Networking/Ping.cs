using System;

namespace GoOS.Networking;

public class Ping
{
    public static void Run()
    {
        /*try
        {
            log(ConsoleColor.Red, "1");
            using (var xClient = new DHCPClient())
            {
                // Send a DHCP Discover packet 
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
                log(ConsoleColor.Blue, NetworkConfiguration.CurrentAddress.ToString());
            }

            using (var xClient = new TcpClient())
            {
                log(ConsoleColor.Red, "2");
                try
                {
                    Address ip = new Address( byte.Parse("216"), byte.Parse("14"), byte.Parse("148"),byte.Parse("33")); 
                    log(ConsoleColor.Blue, ip.ToString());
                    xClient.Connect(ip, 12000, 1000);
                    //5.39.84.58
                }
                catch (Exception e)
                {
                    log(ConsoleColor.Red, e.Message);
                }

                log(ConsoleColor.Red, "3");
                
                xClient.Send(Encoding.UTF8.GetBytes("TEST"));
                
            }
        }
        catch (Exception e)
        {
            log(ConsoleColor.Red, "Internal Error:");
            log(ConsoleColor.White, e.ToString());
        }*/

        Console.WriteLine("Ping is not supported for now.");
    }
}