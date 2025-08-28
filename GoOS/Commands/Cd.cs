using System.IO;
using Console = BetterConsole;

namespace GoOS.Commands;

internal class Cd
{
    public static void Run(string fuck)
    {
        try
        {
            var rootf = @"0:\";
            var cdir = Directory.GetCurrentDirectory();
            Kernel.olddir = cdir;
            // this fuck = fuck.Split("cd ")[1];
            if (fuck.Contains(@"0:\")) fuck.Replace(@"0:\", "");

            if (!fuck.Contains("\\") && fuck != rootf) fuck = "\\" + fuck;

            // this too fuck = fuck.Split("cd ")[1];
            if (Directory.Exists(Directory.GetCurrentDirectory() + fuck))
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + fuck);
        }
        catch
        {
            Console.WriteLine("\nDirectory not found\n");
        }
    }
}