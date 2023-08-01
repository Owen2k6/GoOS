using System;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;
using static ConsoleColorEx;

namespace ChaOS
{
    public class Core
    {
        public static void log(string text = null) => GoOS.GUI.Apps.ChaOS_VM.VMTERM.WriteLine(text);
        public static void clog(string text, ConsoleColor ForeColor)
        {
            var OldFore = GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor;
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor = ForeColor;
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.WriteLine(text);
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor = OldFore;
        }
        public static void write(string text) => GoOS.GUI.Apps.ChaOS_VM.VMTERM.Write(text);
        public static void cwrite(string text, ConsoleColor Color)
        {
            var OldColor = GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor;
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor = Color;
            write(text);
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor = OldColor;
        }

        public static void SetScreenColor(ConsoleColor BackColor, ConsoleColor ForeColor, bool ClearScreen = true)
        {
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.BackgroundColor = BackColor; GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor = ForeColor;
            if (ClearScreen) GoOS.GUI.Apps.ChaOS_VM.VMTERM.Clear();
        }

        public static void Crash(Exception exc, bool isFatal = false)
        {
            ConsoleColor OldFore = GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor; ConsoleColor OldBack = GoOS.GUI.Apps.ChaOS_VM.VMTERM.BackgroundColor;
            SetScreenColor(DarkBlue, White); GoOS.GUI.Apps.ChaOS_VM.VMTERM.Beep();
            GoOS.GUI.Apps.ChaOS_VM.VMTERM.CursorTop = 10; log("              ChaOS has hit a brick wall and died in the wreckage!\n");
            try { GoOS.GUI.Apps.ChaOS_VM.VMTERM.CursorLeft = 39 - (exc.Message.Length / 2); } catch { GoOS.GUI.Apps.ChaOS_VM.VMTERM.CursorLeft = 0; }
            write(exc.ToString() + "\n\n");

            if (!isFatal)
            {
                write("                          Press any key to continue... ");
                GoOS.GUI.Apps.ChaOS_VM.VMTERM.ReadKey(true); SetScreenColor(OldBack, OldFore);
            }
            else
            {
                write("                                You can restart. ");
                while (true) GoOS.GUI.Apps.ChaOS_VM.VMTERM.ReadKey(true);
            }
        }
    }
}
