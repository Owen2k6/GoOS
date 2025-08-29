using System;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Desktop : Window
{
    // private readonly string[] contextMenuButtons =
    // {
    //     " Garbage Collect "
    // };
    public Desktop()
    {
        Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 19));
        Contents.DrawImage(0, 0, background, false);
        Title = nameof(Desktop);
        Visible = true;
        Closable = false;
        HasTitlebar = false;
        Unkillable = true;
        SetDock(WindowDock.Desktop);


        if (Kernel.BuildType != "R")
        {
            if (Kernel.BuildType == "NIFPR")
            {
                var line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                var line2 =
                    "Shh lets not leak our hard work";
                var line3 =
                    "Despite this being open source, Lets keep the new features as secret as possible";
                var line4 = "Thank you to everyone that is actively developing and testing GoOS";

                Contents.DrawString(Contents.Width - Geneva.MeasureString(line1) - 1, Contents.Height - 53, line1,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line2) - 1, Contents.Height - 41, line2,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line3) - 1, Contents.Height - 29, line3,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line4) - 1, Contents.Height - 17, line4,
                    Geneva,
                    Color.White);
            }
            else if (Kernel.BuildType == "PRB")
            {
                var line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                var line2 =
                    "This Build is not stable and is not recommended for use by Non-Testers";
                var line3 =
                    "Official use of this build type is limited to testers only.";
                var line4 = "GoOS Update and Security are not available for these builds.";

                Contents.DrawString(Contents.Width - Geneva.MeasureString(line1) - 1, Contents.Height - 53, line1,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line2) - 1, Contents.Height - 41, line2,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line3) - 1, Contents.Height - 29, line3,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line4) - 1, Contents.Height - 17, line4,
                    Geneva,
                    Color.White);
            }
            else if (Kernel.BuildType == "PRE")
            {
                var line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                var line2 =
                    "This is a Pre Release of GoOS";
                var line3 =
                    "We don't recommend using this build for regular use.";
                var line4 = "This build sports beta functions that may not be included in the final release.";

                Contents.DrawString(Contents.Width - Geneva.MeasureString(line1) - 1, Contents.Height - 53, line1,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line2) - 1, Contents.Height - 41, line2,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line3) - 1, Contents.Height - 29, line3,
                    Geneva,
                    Color.White);
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line4) - 1, Contents.Height - 17, line4,
                    Geneva,
                    Color.White);
            }
            else if (Kernel.BuildType == "ITB")
            {
                var line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line1) - 1, Contents.Height - 17, line1,
                    Geneva,
                    Color.White);
            }
            else
            {
                var line = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                Contents.DrawString(Contents.Width - Geneva.MeasureString(line) - 1, Contents.Height - 17, line,
                    Geneva,
                    Color.White);
            }
        }
    }

    // public override void ShowContextMenu()
    // {
    //     if (MouseManager.X != 0 && MouseManager.Y != 0) ContextMenu.Show(contextMenuButtons, 155, ContextMenu_Handle);
    // }
    //
    // private void ContextMenu_Handle(string item)
    // {
    //     switch (item)
    //     {
    //         case " Rubbish Collect ":
    //             Dialogue.Show("Garbage Collection", Heap.Collect() + " bytes freed");
    //             break;
    //     }
    // }
}