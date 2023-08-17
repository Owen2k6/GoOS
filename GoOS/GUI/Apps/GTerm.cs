using Cosmos.System;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps
{
    public class GTerm : Window
    {
        public GTerm(bool overrideTitle = true)
        {
            Contents = BetterConsole.Canvas;
            Title = BetterConsole.Title;
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);
            BetterConsole.font = new Font(BetterConsole.rawFont, BetterConsole.charHeight);
            BetterConsole.Visible = true;
            if (overrideTitle)
                BetterConsole.Title = "GTerm";
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
