using System;
using System.IO;
using System.Linq;
using GoOS.Themes;
using static GoOS.Core;

namespace GoOS.Commands;

public class GoCodeInstaller
{
    public static void Install(string file)
    {
        try
        {
            ExtendedFilesystem.CopyFile(file, @"0:\content\GCI\");


            if (file.Contains(".gexe"))
            {
                if (file.Contains("\\"))
                {
                    //Console.WriteLine("3");

                    var whatToRemove = file.Substring(file.LastIndexOf("\\"));

                    var FullName = file.Replace(whatToRemove, "");

                    var name = FullName.Replace(".gexe", "");

                    var location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else
                {
                    //Console.WriteLine("4");

                    var FullName = file;

                    var name = FullName.Replace(".gexe", "");

                    var location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
            }
            else if (file.Contains(".goexe"))
            {
                if (file.Contains("\\"))
                {
                    var whatToRemove = file.Substring(file.LastIndexOf("\\"));

                    var FullName = file.Replace(whatToRemove, "");

                    var name = FullName.Replace(".goexe", "");

                    var location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else
                {
                    var FullName = file;

                    var name = FullName.Replace(".goexe", "");

                    var location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
            }
        }
        catch (Exception e)
        {
            log(ThemeManager.ErrorText, "Error whilst trying to install file: " + e);
        }
    }

    public static void CheckForInstalledPrograms()
    {
        try
        {
            var firectories = File.ReadAllLines(@"0:\content\sys\path.ugms");

            foreach (var firectory in firectories)
                if (!Kernel.pathPaths.Contains(firectory))
                    Kernel.pathPaths.Append(firectory);

            foreach (var pathDir in Kernel.pathPaths)
            {
                var directory_list = Directory.GetFiles(pathDir);

                foreach (var file in directory_list)
                    if (file.EndsWith(".gexe"))
                    {
                        var name = file.Replace(".gexe", "");

                        var location = pathDir + @"\" + file;

                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
                    else if (file.EndsWith(".goexe"))
                    {
                        var name = file.Replace(".goexe", "");

                        var location = pathDir + @"\" + file;

                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
                    else if (file.EndsWith(".9xc"))
                    {
                        var name = file.Replace(".9xc", "");

                        var location = pathDir + @"\" + file;

                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
            }
        }
        catch (Exception e)
        {
            log(ThemeManager.ErrorText, "Error whilst trying to detect installed programs: " + e);
        }
    }

    public static void Uninstall(string name)
    {
        if (Kernel.InstalledPrograms.ContainsKey(name))
        {
            var rootass = @"0:\";

            var currentDIRRRRRR = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(rootass);

            Kernel.InstalledPrograms.TryGetValue(name, out var locat);

            var TrueLocat = locat;

            if (locat.Contains(@"0:\")) TrueLocat = TrueLocat.Replace(@"0:\", "");

            File.Delete(TrueLocat);
            Kernel.InstalledPrograms.Remove(name);

            Directory.SetCurrentDirectory(currentDIRRRRRR);
        }
    }
}