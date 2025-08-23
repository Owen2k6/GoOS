using Cosmos.System;
using GoOS;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;

namespace GoOS.GUI.Apps
{
    public class Terminal : Window
    {
        SVGAIITerminal terminal;

        public Terminal()
        {
            terminal = new SVGAIITerminal(640, 480, Resources.TerminalFont, UpdateRequest);
            terminal.WriteLine("Hello, world!");

            Contents = new Canvas(640, 480);
            Title = "Terminal";
            Visible = true;
            Closable = true;
            Unkillable = true;
            SetDock(WindowDock.Auto);
        }

        public override void HandleRun()
        {
        }

        public override void HandleKey(KeyEvent key)
        {
        }

        private void UpdateRequest()
        {
            unsafe
            {
                terminal.Contents.CopyTo(Contents.Internal);
            }
        }
    }
}
