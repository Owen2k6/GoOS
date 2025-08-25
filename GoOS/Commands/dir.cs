using System;
using System.IO;
using GoOS.Themes;
using GoOS.GUI.Apps.Terminal;

namespace GoOS.Commands
{
    internal class Dir
    {
        public static void Run(Shell terminal)
        {
            
            
            int filecount = 0;
            int foldercount = 0;
            string cdir3002 = Directory.GetCurrentDirectory();
            string cdir3003 = @"0:\";
            if (cdir3002.Contains(@"0:\\"))
            {
                cdir3003 = cdir3002.Replace(@"0:\\", @"0:\");
            }

            try
            {
                var directory_list = Directory.GetFiles(cdir3003);
                var directory2_list = Directory.GetDirectories(cdir3003);
                terminal.log(ThemeManager.Default, "\nDirectory listing at " + cdir3003 + "\n");
                foreach (var directory in directory2_list)
                {
                    terminal.log(ThemeManager.Default, "<Dir> " + directory);
                    foldercount++;
                }

                foreach (var file in directory_list)
                {
                    if (file.EndsWith(".gms") && !Kernel.devMode)
                    {
                        terminal.log(ThemeManager.ErrorText, "<System> Protected File");
                    }
                    else
                    {
                        terminal.log(ThemeManager.Default, "<File> " + file);
                    }

                    filecount++;
                }

                terminal.log(ThemeManager.Default, $"\nListed {filecount} files and {foldercount} folders in this directory.\n");
            }
            catch (Exception e)
            {
                terminal.log(ThemeManager.ErrorText, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
            }
        }
    }
}