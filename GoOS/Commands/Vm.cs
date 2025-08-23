using System;
using System.IO;
using GoOS.GUI.Apps;
using WindowManager = GoOS.GUI.WindowManager;

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
                Terminal term = new Terminal();
                WindowManager.AddWindow(term);
                
                Virtualisation.ChaOS.Kernel Kernel = new Virtualisation.ChaOS.Kernel(term);
                Kernel.Start();

                Kernel = null;
            }
        }
    }
}