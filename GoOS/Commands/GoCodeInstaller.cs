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
            string[] firectories = File.ReadAllLines(@"0:\content\sys\path.ugms");

            foreach (var firectory in firectories)
            {
                if (!Kernel.pathPaths.Contains(firectory))
                {
                    Kernel.pathPaths.Append(firectory); 
                    
                }
            }
            
            foreach (var pathDir in Kernel.pathPaths)
            {
                var directory_list = Directory.GetFiles(pathDir);

                foreach (var file in directory_list)
                {
                    if (file.EndsWith(".gexe"))
                    {
                        string name = file.Replace(".gexe", "");

                        string location = pathDir + @"\" + file;

                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
                    else if (file.EndsWith(".goexe"))
                    {
                        string name = file.Replace(".goexe", "");

                        string location = pathDir + @"\" + file;

                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
                    else if (file.EndsWith(".9xc"))
                    {
                        string name = file.Replace(".9xc", "");
                        
                        string location = pathDir + @"\" + file;
                        
                        if (!Kernel.InstalledPrograms.ContainsKey(name))
                            Kernel.InstalledPrograms.Add(name, location);
                    }
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
            string rootass = @"0:\";

            string currentDIRRRRRR = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(rootass);

            Kernel.InstalledPrograms.TryGetValue(name, out string locat);

            string TrueLocat = locat;

            if (locat.Contains(@"0:\"))
            {
                TrueLocat = TrueLocat.Replace(@"0:\", "");
            }

            File.Delete(TrueLocat);
            Kernel.InstalledPrograms.Remove(name);

            Directory.SetCurrentDirectory(currentDIRRRRRR);
        }
    }
}