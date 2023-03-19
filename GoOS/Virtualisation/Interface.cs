using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.ConsoleColor;
using Sys = Cosmos.System;
using GoOS.Virtualisation;
using Cosmos.System.FileSystem.VFS;

namespace GoOS.Virtualisation
{
    //This is not to be used. mainly to create a basic image for developers to clone into their own .cs file and produce a "virtual" OS
    //You will have to modify a ton of shit from the OS in order to work with this interface.
    //A shutdown command is highly reccomended and should issue runmode = false;
    internal class Interface
    {
        public static bool runmode = false;
        public static void boot(string rootpath)
        {
            //BeforeRun bullshit goes here
        }
        public static void run(string rtp) {
            
            while (runmode)
            {
                //run contents in here
            }
        
        }
    }
}
