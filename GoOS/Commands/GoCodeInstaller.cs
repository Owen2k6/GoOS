using System;
using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                    
                    string whatToRemove = file.Substring(file.LastIndexOf("\\"));

                    string FullName = file.Replace(whatToRemove, "");

                    string name = FullName.Replace(".gexe", "");

                    string location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else
                {
                    //Console.WriteLine("4");
                    
                    string FullName = file;

                    string name = FullName.Replace(".gexe", "");

                    string location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
            }
            else if (file.Contains(".goexe"))
            {
                if (file.Contains("\\"))
                {
                    string whatToRemove = file.Substring(file.LastIndexOf("\\"));

                    string FullName = file.Replace(whatToRemove, "");

                    string name = FullName.Replace(".goexe", "");

                    string location = @"0:\content\GCI\" + FullName;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else
                {
                    string FullName = file;

                    string name = FullName.Replace(".goexe", "");

                    string location = @"0:\content\GCI\" + FullName;

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
            var directory_list = Directory.GetFiles(@"0:\content\GCI\");

            foreach (var file in directory_list)
            {
                if (file.EndsWith(".gexe"))
                {
                    string name = file.Replace(".gexe", "");

                    string location = @"0:\content\GCI\" + file;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else if (file.EndsWith(".goexe"))
                {
                    string name = file.Replace(".goexe", "");

                    string location = @"0:\content\GCI\" + file;

                    Kernel.InstalledPrograms.Add(name, location);
                }
                else
                {
                    
                }
            }
        }
        catch (Exception e)
        {
            log(ThemeManager.ErrorText, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
        }
    }
}