using System;
using System.IO;

namespace GoOS.Commands
{
    public class VM
    {
        public static void Run(string args)
        {
            if (!Directory.Exists(@"0:\content\vrt\"))
                Directory.CreateDirectory(@"0:\content\vrt\");

            if (!Directory.Exists(@"0:\content\vrt\ChaOS\"))
                Directory.CreateDirectory(@"0:\content\vrt\ChaOS\");
            
            if (args.Equals("ChaOS", StringComparison.OrdinalIgnoreCase))
            {
                Virtualisation.ChaOS.Kernel Kernel = new Virtualisation.ChaOS.Kernel();
                Kernel.Start();

                Kernel = null;
            }
        }
    }
}