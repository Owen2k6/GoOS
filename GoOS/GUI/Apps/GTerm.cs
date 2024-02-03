using Cosmos.System;
using GoGL.Graphics.Fonts;

namespace GoOS.GUI.Apps
{
    public class GTerm : Window
    {
        public GTerm(bool overrideTitle = true)
        {
            if (overrideTitle) BetterConsole.Title = "GTerm";

            Contents = BetterConsole.Canvas;
            Title = BetterConsole.Title;
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);
            BetterConsole.Visible = true;
        }

        public override void HandleRun()
        {
            if (!BetterConsole.Visible)
            {
                Closing = true;
            }
        }

        public override void HandleKey(KeyEvent key)
        {
            BetterConsole.KeyBuffer.Enqueue(key);
        }
    }
}
