using System.Reflection.Metadata.Ecma335;

namespace GoOS.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = BetterConsole;

public class ExtendedFilesystem
{
    public static void CopyFile(string from, string to)
    {
        if (from.Contains("\\"))
        {   
            //Console.WriteLine("1");
            string whatToRemove = from.Substring(from.LastIndexOf("\\"));

            string FullName = from.Replace(whatToRemove, "");

            string name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllText(from);
            File.Create(to + FullName);
            File.WriteAllText(to + FullName, Contents);
        }
        else
        {
            //Console.WriteLine("2");
            string FullName = from;

            string name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllText(from);
            File.Create(to + FullName);
            File.WriteAllText(to + FullName, Contents);
        }
    }

    public static void MoveFile(string from, string to)
    {
        if (from.Contains("\\"))
        {
            string whatToRemove = from.Substring(from.LastIndexOf("\\"));

            string FullName = from.Replace(whatToRemove, "");

            string name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllText(from);
            File.Create(to + FullName);
            File.WriteAllText(to + FullName, Contents);
            File.Delete(from);
        }
        else
        {
            string FullName = from;

            string name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllText(from);
            File.Create(to + FullName);
            File.WriteAllText(to + FullName, Contents);
            File.Delete(from);
        }
    }
}