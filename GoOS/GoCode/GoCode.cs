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
using Console = GoOS.SVGAIITerminal;
using static Gold.Graphics.Color;

namespace GoOS.GoCode;


public class GoCode
{
    public static string Version = "0.0.2";
    
    public static void Run(Terminal terminal, string file, bool usecurrentdir = true, bool unnecessaryOutputs = true)
    {
        try
        {
            if (unnecessaryOutputs)
            {
                terminal.log(Cyan, "Goplex Studios GoOS GoCode Interpreter\n");
            }
            
            if (!file.EndsWith(".gexe") && !file.EndsWith(".goexe"))
            {
                terminal.log(ThemeManager.ErrorText, "Incompatible format.");
                terminal.log(ThemeManager.ErrorText, "File must be .gexe");
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

                Interpreter GoCodeInterpreter = new Interpreter(terminal);
                
                GoCodeInterpreter.Interpret(content, unnecessaryOutputs);

                GoCodeInterpreter = null;
            }
        }
        catch (Exception e)
        {
            terminal.terminal.WriteLine(e);
            throw;
        }
    }
}