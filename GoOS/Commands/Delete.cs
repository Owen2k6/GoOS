using System;
using System.IO;

namespace GoOS.Commands
{
    internal class Delete
    {
        public static void DeleteDirectory(SVGAIITerminal Console, string args)
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

        public static void DeleteFile(SVGAIITerminal Console, string args)
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
        
        public static void UniversalDelete(SVGAIITerminal Console, string args)
        {
            if (args.Contains("0:\\"))
            {
                args.Replace(@"0:\", "");
            }
        
            String DirArgs = "\\" + args;
        
            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + args))
                File.Delete(Directory.GetCurrentDirectory() + @"\" + args);
            else if (Directory.Exists(Directory.GetCurrentDirectory() + @"\" + args))
                Directory.Delete(Directory.GetCurrentDirectory() + @"\" + args, true);
            else
            {
                Console.WriteLine("File or Directory not found!");
            }
        }
    }
}