using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using GoOS.GUI;
using GoOS.GUI.Apps;
using GoOS.Themes;
using Console = BetterConsole;
using static ConsoleColorEx;
using static GoOS.Core;

namespace GoOS.GoCode;

public class GoCode
{
    public static void Run(string file, bool usecurrentdir = true, bool unnecessaryOutputs = true)
    {
        try
        {
            if (unnecessaryOutputs)
            {
                log(Cyan, "Goplex Studios GoOS GoCode Interpreter\n");
            }
            
            if (!file.EndsWith(".gexe") && !file.EndsWith(".goexe"))
            {
                log(ThemeManager.ErrorText, "Incompatible format.");
                log(ThemeManager.ErrorText, "File must be .gexe");
            }

            if (file.EndsWith(".goexe") || file.EndsWith(".gexe"))
            {
                string[] content;
                if (usecurrentdir)
                {
                    content = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\" + file);
                }
                else
                {
                    content = File.ReadAllLines(file);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}