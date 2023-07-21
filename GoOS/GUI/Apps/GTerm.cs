using Cosmos.System;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps
{
    public class GTerm : Window
    {
        public GTerm()
        {
            Contents = BetterConsole.Canvas;
            X = 20;
            Y = 50;
            Title = "GTerm";
            Visible = true;
            Closable = true;
            BetterConsole.font = new Font(BetterConsole.rawFont, BetterConsole.charHeight);
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
