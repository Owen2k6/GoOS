using System;
using System.IO;
using GoOS.GUI.Apps.Terminal;
using GoOS.Themes;
using static Gold.Graphics.Color;

namespace GoOS.GoCode;


public class GoCode
{
    public static string Version = "0.0.2";
    
    public static void Run(Shell terminal, string file, bool usecurrentdir = true, bool unnecessaryOutputs = true)
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
            terminal._terminal.WriteLine(e);
            throw;
        }
    }
}