using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.Commands
{
    internal class Make
    {
        public static void MakeDirectory(string args)
        {
            if (args.Contains(@"0:\"))
            {
                args.Replace(@"0:\", "");
            }

            //potato = potato.Split("mkdir ")[1];
            if (!Directory.Exists(args))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + args);
        }

        public static void MakeFile(string args)
        {
            if (args.Contains(@"0:\"))
            {
                args.Replace(@"0:\", "");
            }

            //potato2 = potato2.Split("mkfile ")[1];
            if (!File.Exists(args))
                File.Create(Directory.GetCurrentDirectory() + @"\" + args);
        }
    }
}