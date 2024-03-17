using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using GoOS.Commands;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using GoGL.Hardware.GPU;
using GoOS.GUI.Apps.Settings;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        private string[] apps;
        List<Button> sButtons = new();

        private const int buttonHeight = 24;

        /*private void AddAppButton(string name, Action clickedAction)
        {
            appButtons.Add(new Button(this, 8, (ushort)(64 + (appButtons.Count * buttonHeight)), 281, buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }*/
        private void AddSideButton(string name, Action clickedAction)
        {
            sButtons.Add(new Button(this, (ushort)(Contents.Width - 96 - 8),
                (ushort)(64 + (sButtons.Count * buttonHeight)), 96, buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }

        private void AddAppButtons()
        {
            if (!File.Exists(@"0:\content\sys\pinnedapps.gms"))
            {
                File.Create(@"0:\content\sys\pinnedapps.gms");
            }

            apps = File.ReadAllLines(@"0:\content\sys\pinnedapps.gms");
            if (apps.Length == 0)
            {
                Contents.DrawString(8, 64, "TIP: Apps you pin from Gosplorer\nwill appear here", Resources.Font_1x, Color.White);
            }
            Button[] buttons = new Button[apps.Length];

            int offset = 0;

            for (int i = 0; i < apps.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(apps[i].Trim());

                if (File.Exists(apps[i].Trim()))
                {
                    buttons[i] = new Button(this, 8, (ushort)(64 + (i * buttonHeight - offset * buttonHeight)), 281,
                        buttonHeight, name)
                    {
                        Name = apps[i].Trim(),
                        ClickedAlt = appAction
                    };
                }
                else
                {
                    offset++;
                }

                if (buttons[i] != null) buttons[i].Render();
            }
        }

        private void appAction(string path)
        {
            WindowManager.AddWindow(new GTerm());
            BetterConsole.Clear();

            if (!path.EndsWith(".9xc"))
                GoCode.GoCode.Run(path, false, false);
            else
                _9xCode.Interpreter.Run(path);
        }

        private void AddSideButtons()
        {
            AddSideButton("Apps", () =>
            {
                WindowManager.AddWindow(new AppManager());
                Dispose();
            });
            AddSideButton("GoStore", () =>
            {
                WindowManager.AddWindow(new GoStore.MainFrame());
                Dispose();
            });
            /*AddSideButton("Gosplorer", () =>
            {
                WindowManager.AddWindow(new GosplorerOld());
                Dispose();
            });*/
            AddSideButton("Gosplorer", () =>
            {
                WindowManager.AddWindow(new Gosplorer.MainFrame());
                Dispose();
            });
            AddSideButton("GoWeb", () =>
            {
                WindowManager.AddWindow(new GoWeb.GoWebWindow());
                Dispose();
            });
            AddSideButton("Settings", () =>
            {
                WindowManager.AddWindow(new Frame());
                Dispose();
            });
            AddSideButton("Paint", () =>
            {
                WindowManager.AddWindow(new Paintbrush());
                Dispose();
            });
            AddSideButton("Notepad", () =>
            {
                WindowManager.AddWindow(new Notepad(false, null));
                Dispose();
            });
        }

        private void AddPowerButton()
        {
            Button button = new Button(
                this,
                (ushort)(Contents.Width - 96 - 8),
                (ushort)(Contents.Height - 64 - 8),
                96,
                64,
                "Power..."
            );

            button.Image = shutdownIcon;

            button.Clicked = Power_Click;
        }

        public StartMenu()
        {
            //WindowManager.AddWindow(new GTerm());

            Contents = new Canvas(400, 500);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            Title = nameof(StartMenu);
            HasTitlebar = false;
            Visible = true;
            Unkillable = true;

            Contents.DrawImage(0, 0, startMenuBackground);
            Contents.DrawImage(10, 11, userImage);
            Contents.DrawString(52, 28, Kernel.username, Resources.Font_1x, Color.White);

            AddAppButtons();

            AddPowerButton();

            AddSideButtons();

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }

        private void Power_Click()
        {
            //WindowManager.Dimmed = true;
            Dialogue.Show(
                "GoOS",
                "What would you like to do?",
                new()
                {
                    new() { Text = "Shut Down", Callback = () => { Power.Shutdown(); } },

                    new() { Text = "Reboot", Callback = () => { Power.Reboot(); } }
                },
                shutdownIcon
            );

            Visible = false;
        }

        /*public void ToggleStartMenu()
        {
            if (Visible)
            {
                Dispose();
            }
            else
            {
                OpenStartMenu();
            }
        }*/
    }
}