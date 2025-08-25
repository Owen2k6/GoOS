using System;
using Gold.Graphics;

namespace GoOS.GUI.Apps
{
    public class TaskManager : Window
    {
        Button EndButton;
        Button AboutButton;
        List Windows;

        public static bool pko = false;

        public TaskManager() : base(0, 0, 270, 310, "Task Manager")
        {
            Contents = new Canvas(270, 310);
            Title = "Task Manager";
            //Visible = true;
            //Closable = true;
            //Unkillable = true;
            SetDock(WindowDock.Auto);

            EndButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30),
                80, 20, " End task ") { Clicked = EndButton_Click };
            Windows = new List(this, 10, 10, Convert.ToUInt16(Contents.Width - 20),
                Convert.ToUInt16(Contents.Height - 60), "Processes", Array.Empty<string>());

            // Render the buttons.
            Contents.Clear(Color.LightGray);
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40),
                Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
            AboutButton.Render();
            EndButton.Render();
        }

        private void Update()
        {
            Windows.Items = new string[WindowManager.Windows.Count]; // Reallocate array size.
            for (int i = 0; i < Windows.Items.Length; i++)
                Windows.Items[i] =
                    WindowManager.Windows[i].Title; // Copy the title from the windows array to the items array.

            Windows.Render(); // Render the window list.
        }

        private void EndButton_Click()
        {
            if (!pko && !Kernel.devMode)
            {
                Dialogue.Show(
                    "Error",
                    "System processes are not\nendable.",
                    null,
                    Resources.errorIcon);
            }
            else
            {
                WindowManager.Windows[Windows.Selected].Closing = true; // Close the window.
            }
        }
    }
}