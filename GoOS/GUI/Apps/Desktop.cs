using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class Desktop : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.folder.bmp")] private static byte[] folderIconRaw;
        private static Canvas folderIcon = Image.FromBitmap(folderIconRaw, false);

        Button FolderButton;

        public Desktop()
        {
            Fonts.Generate();

            Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 28));
            Contents.Clear(Kernel.DesktopColour);
            Title = nameof(Desktop);
            Visible = true;
            Closable = false;
            HasTitlebar = false;
            Unkillable = true;
            SetDock(WindowDock.None);

            FolderButton = new Button(this, 20, 20, 64, 80, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = Kernel.DesktopColour,
                TextColour = Color.White,

                Image = folderIcon,
                Clicked = FolderButton_Click
            };

            FolderButton.Render();

            Contents.DrawString(Contents.Width - Fonts.Font_1x.MeasureString("GoOS " + Kernel.BuildType + " " + Kernel.version) - 22, Contents.Height - 42, "GoOS " + Kernel.BuildType + " " + Kernel.version, Fonts.Font_1x, Color.White);
            Contents.DrawString(Contents.Width - Fonts.Font_1x.MeasureString("Development build") - 22, Contents.Height - 25, "Development build", Fonts.Font_1x, Color.White);
        }

        private void FolderButton_Click()
        {
            WindowManager.AddWindow(new AppManager());
        }
    }
}
