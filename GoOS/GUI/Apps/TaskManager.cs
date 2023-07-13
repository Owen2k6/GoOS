using System;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class TaskManager : Window
    {
        Button EndButton;
        Button AboutButton;
        List Windows;

        public TaskManager()
        {
            Contents = new Canvas(270, 310);
            X = 200;
            Y = 100;
            Title = "Task Manager";
            Visible = true;
            Closable = true;

            EndButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, " End task ") { Clicked = EndButton_Click };
            AboutButton = new Button(this, Convert.ToUInt16(Contents.Width - 124), Convert.ToUInt16(Contents.Height - 30), 24, 20, "?") { Clicked = AboutButton_Click };
            Windows = new List(this, 10, 10, Convert.ToUInt16(Contents.Width - 20), Convert.ToUInt16(Contents.Height - 60), "Processes", Array.Empty<string>());

            WindowManager.TaskmanHook = Update;

            // Render the buttons.
            Contents.Clear(Color.White);
            RenderSystemStyleBorder();
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, new Color(234, 234, 234));
            AboutButton.Render();
            EndButton.Render();
        }

        private void Update()
        {
            Windows.Items = new string[WindowManager.windows.Count]; // Reallocate array size.
            for (int i = 0; i < Windows.Items.Length; i++)
                Windows.Items[i] = WindowManager.windows[i].Title; // Copy the title from the windows array to the items array.

            Windows.Render(); // Render the window list.
        }

        private void EndButton_Click()
        {
            if (WindowManager.windows[Windows.Selected].Unkillable)
            {
                Dialogue.Show(
                    "Warning",
                    "System processes are not\nendable.",
                    null,
                    WindowManager.warningIcon);
            }
            else
            {
                WindowManager.windows[Windows.Selected].Closing = true; // Close the window.
            }
        }

        private void AboutButton_Click()
        {
            Dialogue.Show(
                "About Task Manager",
                $"GoOS Task Manager v{Kernel.version}\n\nCopyright (c) 2023 Owen2k6\nAll rights reserved.",
                null);
        }
    }
}
