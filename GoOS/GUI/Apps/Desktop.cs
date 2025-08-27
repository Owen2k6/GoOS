using Cosmos.Core.Memory;
using Cosmos.System;
using Gold.Graphics;
using GoOS.Tasking;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class Desktop : Window
    {
        Button AppsFolderButton;

        private string[] contextMenuButtons =
        {
            " Garbage Collect ",
        };

        public override void ShowContextMenu()
        {
            if (MouseManager.X != 0 && MouseManager.Y != 0)
            {
                ContextMenu.Show(contextMenuButtons, 155, ContextMenu_Handle);
            }
        }

        private void ContextMenu_Handle(string item)
        {
            switch (item)
            {
                case " Garbage Collect ":
                    Dialogue.Show("Garbage Collection", Heap.Collect() + " bytes freed");
                    break;
            }
        }

        public Desktop() : base(0, 0, WindowManager.Screen.Width, WindowManager.Screen.Height, "Desktop")
        {
            //Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 28));
            //Contents.Clear(Kernel.DesktopColour);
            Contents.DrawImage(0, 0, background, false);
            SetDock(WindowDock.None);

            if (Kernel.BuildType != "R")
            {
                if (Kernel.BuildType == "NIFPR")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "Shh lets not leak our hard work.";
                    string line3 =
                        "Despite this being open source, Lets keep the new features as secret as possible.";
                    string line4 = "Thank you to everyone that is actively developing and testing GoOS.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "PRB")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "This Build is not stable and is not recommended for use by Non-Testers";
                    string line3 =
                        "Official use of this build type is limited to testers only.";
                    string line4 = "GoOS Update and Security are not available for these builds.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "PRE")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "This is a Pre Release of GoOS";
                    string line3 =
                        "We don't recommend using this build for regular use.";
                    string line4 = "This build sports beta functions that may not be included in the final release.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "INTERNAL TEST BUILD")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 17, line1,
                        Font_1x,
                        Color.White);
                }
                else
                {
                    string line = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line) - 1, Contents.Height - 17, line,
                        Font_1x,
                        Color.White);
                }
            }
        }

        internal override void Render()
        {
            //Contents.DrawImage(0, 0, background, false);
            
            //Contents.DrawString(200, 46, WindowManager.FPS + " FPS", Resources.Charcoal, Color.White, Shadow: true);
            
            // This generates MASSIVE lag spikes; only use if you really need to debug this sort of stuff
            /*Contents.DrawString(200, 88, "Process list:", Resources.Charcoal, Color.White, Shadow: true);
            for (int i = 0; i < ProcessScheduler.Processes.Count; i++)
                Contents.DrawString(150, 114 + (i * 16), ProcessScheduler.Processes[i].PID + " (" + ProcessScheduler.Processes[i].Name + ")",
                    Resources.Charcoal, Color.White, Shadow: true);

            Contents.DrawString(400, 88, "Window list:", Resources.Charcoal, Color.White, Shadow: true);
            for (int i = 0; i < WindowManager.Windows.Count; i++)
                Contents.DrawString(400, 114 + (i * 16), WindowManager.Windows[i].PID + " (" + WindowManager.Windows[i].Name + ")",
                    Resources.Charcoal, Color.White, Shadow: true);*/
        }
        
        internal override void HandleRun()
        {
            Render();
            
            foreach (Control c in Controls)
            {
                if (c == null) Controls.Remove(c);
                else c.HandleRun();
            }
            
            if (MouseManager.LastMouseState != MouseManager.MouseState && IsMouseOver()) WindowManager.FocusedWindow = this;
        }
    }
}