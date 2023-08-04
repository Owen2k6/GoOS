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

        Button folderButton;
        ContextMenu context;

        public Desktop()
        {
            Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 28));
            Contents.Clear(Kernel.DesktopColour);
            Title = nameof(Desktop);
            Visible = true;
            Closable = false;
            HasTitlebar = false;
            Unkillable = true;
            SetDock(WindowDock.None);

            folderButton = new Button(this, 20, 20, 64, 80, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = Kernel.DesktopColour,
                TextColour = Color.White,

                Image = folderIcon
            };
            
            folderButton.Clicked = FolderClicked;

            folderButton.Render();
        }

        string[] test =
        {
            "Item 1",
            "Item 2"
        };

        // xrc2, listen closely. PULL before doing anything you idiot

        public override void ShowContextMenu()
        {
            ContextMenu.Show(test, 80, ContextMenu_Handle);
        }

        private void ContextMenu_Handle(string item)
        {
            Dialogue.Show(
                    "Debug",
                    item,
                    null);
        }

        private static void FolderClicked()
        {
            if (!WindowManager.Dimmed)
                WindowManager.AddWindow(new AppManager());
        }
    }
}
