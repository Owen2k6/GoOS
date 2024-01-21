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
        List<Button> sButtons = new();

        private const int buttonHeight = 24;

        private void AddAppButton(string name, Action clickedAction)
        {
            appButtons.Add(new Button(this, 8, (ushort)(64 + (appButtons.Count * buttonHeight)), 281, buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }
        private void AddSideButton(string name, Action clickedAction)
        {
            sButtons.Add(new Button(this, (ushort)(Contents.Width - 96 - 8), (ushort)(64 + (sButtons.Count * buttonHeight)), 96, buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }

        private void AddAppButtons()
        {

            AddAppButton("ToDO: Click me!", () =>
            {
                Dialogue.Show("GoOS", "We should have a way of allowing users to \"pin\" apps here");
                CloseStartMenu();
            });
        }
        
        private void AddSideButtons()
        {
            AddSideButton("Apps", () =>
            {
                WindowManager.AddWindow(new AppManager());
                CloseStartMenu();
            });
            AddSideButton("GoStore", () =>
            {
                WindowManager.AddWindow(new GoStore.MainFrame());
                CloseStartMenu();
            });
            AddSideButton("Gosplorer", () =>
            {
                WindowManager.AddWindow(new GosplorerOld());
                CloseStartMenu();
            });
            AddSideButton("Gosplorer 2", () =>
            {
                WindowManager.AddWindow(new Gosplorer.MainFrame());
                CloseStartMenu();
            });
            AddSideButton("Settings", () =>
            {
                Dialogue.Show("GoOS", "This feature is not yet implemented.");
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
            Contents = new Canvas(400, 500);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            Title = nameof(StartMenu);
            HasTitlebar = false;
            Visible = false;
            Unkillable = true;

            Contents.DrawImage(0,0, startMenuBackground);
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