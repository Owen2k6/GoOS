using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.Commands
{
    internal class Cd
    {
        public static void Run(string fuck) {
            try
            {
                string rootf = @"0:\";
                string cdir = Directory.GetCurrentDirectory();
                Kernel.olddir = cdir;
                // this fuck = fuck.Split("cd ")[1];
                if (fuck.Contains(@"0:\")) { fuck.Replace(@"0:\", ""); }
                if (!fuck.Contains("\\") && fuck != rootf) { fuck = "\\" + fuck; }
                // this too fuck = fuck.Split("cd ")[1];
                if (Directory.Exists(Directory.GetCurrentDirectory() + fuck))
                    Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + fuck);
            }
            catch {
                Console.WriteLine("\nDirectory not found\n");
            }
            }
    }
}
