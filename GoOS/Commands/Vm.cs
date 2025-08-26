using System;
using System.IO;
using WindowManager = GoOS.GUI.WindowManager;
using GoOS.GUI.Apps.Terminal;

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
                GoTerminal term = new GoTerminal();
                WindowManager.AddWindow(term);
                
                Virtualisation.ChaOS.Kernel Kernel = new Virtualisation.ChaOS.Kernel(term.term._shell);
                Kernel.Start();

                Kernel = null;
            }
        }
    }
}