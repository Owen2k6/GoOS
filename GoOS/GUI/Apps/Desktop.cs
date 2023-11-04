using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class Desktop : Window
    {
        Button FolderButton;

        public Desktop()
        {
            Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 28));
            //Contents.Clear(Kernel.DesktopColour);
            Contents.DrawImage(0, 0, background, false);
            Title = nameof(Desktop);
            Visible = true;
            Closable = false;
            HasTitlebar = false;
            Unkillable = true;
            SetDock(WindowDock.None);

            FolderButton = new Button(this, 20, 20, 64, 80, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = folderIcon,
                Clicked = FolderButton_Click
            };

            FolderButton.Render();
            string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
            string line2 = "Development build";

            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1)-1, Contents.Height - 29, line1, Font_1x, Color.White);
            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2)-1, Contents.Height - 17, line2, Font_1x, Color.White);
        }

        private void FolderButton_Click()
        {
            WindowManager.AddWindow(new AppManager());
        }
    }
}
