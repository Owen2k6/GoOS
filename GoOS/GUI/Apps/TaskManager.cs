using System;
using Gold.Graphics;

namespace GoOS.GUI.Apps
{
    public class TaskManager : Window
    {
        Button EndButton;
        Button AboutButton;
        List Tasks;

        public static bool pko = false;

        public TaskManager() : base(0, 0, 270, 310, "Task Manager")
        {
            //Visible = true;
            //Closable = true;
            //Unkillable = true;
            SetDock(WindowDock.Auto);

            EndButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30),
                80, 20, " End task ") { Clicked = EndButton_Click };
            Tasks = new List(this, 10, 10, Convert.ToUInt16(Contents.Width - 20),
                Convert.ToUInt16(Contents.Height - 60), "Processes", Array.Empty<string>());

            // Render the buttons.
            Contents.Clear(Color.LightGray);
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40),
                Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
            
            Render();
        }

        internal override void Render()
        {
            Tasks.Items = new string[Kernel.ProcessScheduler.Processes.Count]; // Reallocate array size.
            for (int i = 0; i < Tasks.Items.Length; i++)
                Tasks.Items[i] =
                    Kernel.ProcessScheduler.Processes[i].Name; // Copy the title from the windows array to the items array.

            base.Render();
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
                Kernel.ProcessScheduler.Processes[Tasks.Selected].Dispose();
            }
        }
    }
}