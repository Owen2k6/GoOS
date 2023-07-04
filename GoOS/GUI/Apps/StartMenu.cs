using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.user.bmp")] private static byte[] userImageRaw;
        private static Canvas userImage = Image.FromBitmap(userImageRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.shutdown.bmp")] private static byte[] shutdownIconRaw;
        private static Canvas shutdownIcon = Image.FromBitmap(shutdownIconRaw, false);

        List<Button> appButtons = new();

        private const int buttonHeight = 32;

        private void AddAppButton(string name, Action clickedAction)
        {
            appButtons.Add(new Button(this, 8, (ushort)(48 + (appButtons.Count * buttonHeight)), (ushort)(Contents.Width - 16), buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }

        private void AddAppButtons()
        {
            AddAppButton("GoOS Applications", () => {
                WindowManager.AddWindow(new AppManager());
                Visible = false;
            });

            AddAppButton("GTerm", () => {
                WindowManager.AddWindow(new GTerm());
                Visible = false;
            });

            AddAppButton("Clock", () =>
            {
                WindowManager.AddWindow(new Clock());
                Visible = false;
            });
        }

        private void AddPowerButton()
        {
            Button button = new Button(
                this,
                (ushort)(Contents.Width - 96 - 20),
                (ushort)(Contents.Height - 64 - 20),
                96,
                64,
                "Shut Down"
            );

            button.Image = shutdownIcon;

            button.Clicked = Power_Click;
        }

        public StartMenu()
        {
            Contents = new Canvas(256, 384);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            HasTitlebar = false;
            Visible = false;

            Contents.DrawImage(8, 8, userImage);
            Contents.DrawString(40, 16, Kernel.username, BetterConsole.font, Color.White);

            AddAppButtons();

            AddPowerButton();

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }

        private void Power_Click()
        {
            Dialogue.Show(
                "GoOS",
                "What would you like to do?", new()
                {
                    new() { Text = "Shut Down", Callback = () =>
                    {
                        Power.Shutdown();
                    }},

                    new() { Text = "Reboot", Callback = () =>
                    {
                        Power.Reboot();
                    }}
                }
            );
        }
    }
}
