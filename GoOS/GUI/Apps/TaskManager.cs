using System;
using Cosmos.System;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class TaskManager : Window
    {
        Button UpdateButton;
        Input  WindowToKill;

        public TaskManager()
        {
            Contents = new Canvas(200, Convert.ToUInt16(60 + WindowManager.windows.Count * 20));
            X        = 200;
            Y        = 100;
            Title    = "Task Manager";
            Visible  = true;
            Closable = true;

            UpdateButton = new Button(this, Convert.ToUInt16(Contents.Width - 22), Convert.ToUInt16(Contents.Height - 22), 20, 20, "X") { Clicked = KillButton_Click };
            WindowToKill = new Input (this, 2, Convert.ToUInt16(Contents.Height - 21), Convert.ToUInt16(Contents.Width - 25), 20, "Window to kill");

            RenderWindowList();
        }

        public override void HandleKey(KeyEvent key)
        {
            base.HandleKey(key);

            switch (key.Key)
            {
                case ConsoleKeyEx.F5:
                    Refresh();
                    break;

                case ConsoleKeyEx.Enter:
                    KillButton_Click();
                    break;
            }
        }

        private void KillButton_Click()
        {
            WindowManager.windows[Convert.ToInt32(WindowToKill.Text) - 1].Closing = true; /* Start from 1 instead of 0 */
            Refresh();
        }

        private void Refresh()
        {
            Dispose();
            WindowManager.AddWindow(new TaskManager());
        }

        private void RenderWindowList()
        {
            Contents.Clear(Color.White);
            Contents.DrawString(2, 2, "Active Windows:", BetterConsole.font, Color.Black);

            for (int i = 0; i < WindowManager.windows.Count; i++)
            {
                if (WindowManager.windows[i] != null)
                {
                    Contents.DrawString(2, Convert.ToUInt16(20 + (i * 20)), $"{i + 1 /* Same thing here */}. {WindowManager.windows[i].Title.Trim()}", BetterConsole.font, Color.Black);
                }
            }

            UpdateButton.Render();
            WindowToKill.Render();
        }
    }
}
