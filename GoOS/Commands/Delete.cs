using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.Commands
{
    internal class Delete
    {
        public static void DeleteDirectory(string args)
        {
            if (args.Contains(@"0:\"))
            {
                args.Replace(@"0:\", "");
            }

            args = "\\" + args;
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\" + args))
                Directory.Delete(Directory.GetCurrentDirectory() + @"\" + args, true);
            else if (!Directory.Exists(args))
            {
                Console.WriteLine("Directory does not exist.");
            }
        }

        public static void DeleteFile(string args)
        {
            if (args.Contains("0:\\"))
            {
                args.Replace(@"0:\", "");
            }

            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + args))
                File.Delete(Directory.GetCurrentDirectory() + @"\" + args);
            else if (!File.Exists(args))
            {
                Console.WriteLine("File does not exist.");
            }

            ;
        }
    }
}