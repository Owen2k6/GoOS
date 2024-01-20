using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.Hardware.GPU;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        List<Button> appButtons = new();

        private const int buttonHeight = 24;

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
                CloseStartMenu();
            });

            AddAppButton("Paint", () =>
            {
                WindowManager.AddWindow(new Paintbrush());
                CloseStartMenu();
            });

            AddAppButton("Notepad", () =>
            {
                WindowManager.AddWindow(new Notepad(false, ""));
                CloseStartMenu();
            });

            AddAppButton("GCLauncher", () =>
            {
                WindowManager.AddWindow(new GCLauncher());
                CloseStartMenu();
            });

            AddAppButton("Gosplorer", () =>
            {
                WindowManager.AddWindow(new Gosplorer());
                CloseStartMenu();
            });

            AddAppButton("GoIDE", () =>
            {
                WindowManager.AddWindow(new GoIDE.ProjectsFrame());
                CloseStartMenu();
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
            Contents = new Canvas(256, 384);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            Title = nameof(StartMenu);
            HasTitlebar = false;
            Visible = false;
            Unkillable = true;

            Contents.DrawImage(8, 8, userImage);
            Contents.DrawString(40, 16, Kernel.username, Resources.Font_1x, Color.White);

            AddAppButtons();

            AddPowerButton();

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }

        private void Power_Click()
        {
            WindowManager.Dimmed = true;
            Dialogue.Show(
                "GoOS",
                "What would you like to do?",
                new()
                {
                    new() { Text = "Shut Down", Callback = () =>
                    {
                        Power.Shutdown();
                    }},

                    new() { Text = "Reboot", Callback = () =>
                    {
                        Power.Reboot();
                    }}
                },
                shutdownIcon
            );

            Visible = false;
        }

        private void OpenStartMenu()
        {
            Visible = true;

            WindowManager.MoveWindowToFront(this);

            WindowManager.GetWindowByType<Taskbar>().HandleStartMenuOpen();
        }

        private void CloseStartMenu()
        {
            Visible = false;

            WindowManager.GetWindowByType<Taskbar>().HandleStartMenuClose();
        }

        public void ToggleStartMenu()
        {
            if (Visible)
            {
                CloseStartMenu();
            }
            else
            {
                OpenStartMenu();
            }
        }
    }
}