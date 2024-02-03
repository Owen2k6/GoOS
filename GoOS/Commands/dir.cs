﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoOS.Themes;
using static GoOS.Core;

namespace GoOS.Commands
{
    internal class Dir
    {
        public static void Run()
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
                log(ThemeManager.Default, "\nDirectory listing at " + cdir3003 + "\n");
                foreach (var directory in directory2_list)
                {
                    log(ThemeManager.Default, "<Dir> " + directory);
                    foldercount++;
                }

                foreach (var file in directory_list)
                {
                    if (file.EndsWith(".gms") && !Kernel.devMode)
                    {
                        log(ThemeManager.ErrorText, "<System> Protected File");
                    }
                    else
                    {
                        log(ThemeManager.Default, "<File> " + file);
                    }

                    filecount++;
                }

                log(ThemeManager.Default, $"\nListed {filecount} files and {foldercount} folders in this directory.\n");
            }
            catch (Exception e)
            {
                log(ThemeManager.ErrorText, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
            }
        }
    }
}