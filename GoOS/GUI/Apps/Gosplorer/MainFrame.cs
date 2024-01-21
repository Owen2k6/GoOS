using System;
using System.IO;
using System.Linq;
using GoOS.Commands;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.Gosplorer
{
    public class MainFrame : Window
    {
        const int IconWidth = 64, IconHeight = 80;
        string Path = @"0:\";

        Input AddressBar;
        Button BackButton;
        Button ForwardButton;
        Button UpButton;

        Button[] Shortcuts;
        Button[] FolderContents;

        public MainFrame()
        {
            Contents = new Canvas(480, 360);
            Title = Path + " - Gosplorer";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            AddressBar = new Input(this, 90, 10, 380, 20, "Path") { /*Submitted = AddressBar_Submit*/ }; // TODO: submitted is probably broken
            //AddressBar.Text = Path; // TODO: comment out if it freezes
            BackButton = new Button(this, 10, 10, 20, 20, string.Empty) { Image = arrowleft };
            ForwardButton = new Button(this, 30, 10, 20, 20, string.Empty) { Image = arrowright };
            UpButton = new Button(this, 60, 10, 20, 20, string.Empty) { Image = arrowup };

            Shortcuts = new Button[]
            {
                new Button(this, 26, 45, 40, 20, "0:/")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                },
                new Button(this, 26, 70, 40, 20, "1:/")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                },
                new Button(this, 26, 95, 48, 20, "Apps")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                }
            };

            AddressBar_Submit();
        }

        public override void Paint()
        {
            BetterConsole.WriteLine("[ DEBUG ] Entered void Paint()");

            Contents.DrawImage(0, 0, appbackground, false);
            Contents.DrawImage(0, 0, header, false);
            Contents.DrawImage(0, 40, sidebar, false);
            Contents.DrawImage(10, 45, drive);
            Contents.DrawImage(10, 70, drive_locked);
            Contents.DrawImage(10, 95, ideIconSmall);

            //AddressBar.Render(); fuck this shit
            BackButton.Render();
            ForwardButton.Render();
            UpButton.Render();

            foreach (Button i in Shortcuts) i.Render();
            foreach (Button i in FolderContents) i.Render();

            RenderSystemStyleBorder();
        }

        private void AddressBar_Submit()
        {
            BetterConsole.WriteLine("[ DEBUG] Entered void AddressBar_Submit()");

            string[] itemNames = Directory.GetDirectories(Path).Concat(Directory.GetFiles(Path)).ToArray();
            bool[] itemTypes = itemNames.Select(item => Directory.Exists(item)).ToArray();
            int row = 0, column = 0;

            foreach (Button i in FolderContents)
            {
                BetterConsole.WriteLine("[ DEBUG ] Removing " + i.Title + "(" + i.Name + ")");
                Controls.Remove(i);
            }
            FolderContents = new Button[itemNames.Length];
            for (int i = 0; i < itemNames.Length; i++)
            {
                // TODO: this crashes it
                //if (itemNames[i].EndsWith(".gms"))
                //{
                //    i--;
                //    continue;
                //}

                if (column >= 5)
                {
                    column = 0;
                    row++;
                }

                FolderContents[i] = new Button(this, (ushort)(94 + (column * (IconWidth + 10))), (ushort)(50 + (row * (IconHeight + 10))), IconWidth, IconHeight, itemNames[i])
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                    Image = itemTypes[i] ? folderIcon : fileIcon,
                    ClickedAlt = FolderContents_Clicked,
                    Name = itemNames[i]
                };

                column++;
            }

            Paint();
        }

        private void FolderContents_Clicked(string e)
        {
            BetterConsole.WriteLine("[ DEBUG ] " + Path + (Path.EndsWith(@"\") ? "" : @"\") + e);
            BetterConsole.WriteLine("[ DEBUG ] " + Directory.Exists(Path + (Path.EndsWith(@"\") ? "" : @"\") + e).ToString());

            if (Directory.Exists(Path + (Path.EndsWith(@"\") ? "" : @"\") + e))
            {
                BetterConsole.WriteLine("[ DEBUG ] Directory exists!");
                Path += (Path.EndsWith(@"\") ? "" : @"\") + e;
                BetterConsole.WriteLine("[ DEBUG ] Path added to!");
                //AddressBar.Text = Path;
                BetterConsole.WriteLine("[ DEBUG ] Address bar text set!");
                AddressBar_Submit();
                return; // TODO: this may get called even if the if statement is never reached
            }

            switch (Path + (Path.EndsWith(@"\") ? "" : @"\") + e) // TODO: this doesnt work
            {
                case { } a when a.EndsWith(".txt") || a.EndsWith(".gtheme"):
                    WindowManager.AddWindow(new Notepad(true, e));
                    break;

                case { } a when a.EndsWith(".gexe") || a.EndsWith(".goexe"):
                    BetterConsole.Clear();
                    BetterConsole.Title = "GoCode Interpreter";
                    WindowManager.AddWindow(new GTerm(false));

                    Run.Main(e, false);

                    WindowManager.RemoveWindowByTitle("GoCode Interpreter");
                    BetterConsole.Title = "GTerm";
                    BetterConsole.Clear();
                    Kernel.DrawPrompt();
                    break;

                case { } a when a.EndsWith(".9xc"):
                    BetterConsole.Clear();
                    BetterConsole.Title = "9xCode Interpreter";
                    WindowManager.AddWindow(new GTerm(false));

                    _9xCode.Interpreter.Run(e);

                    WindowManager.RemoveWindowByTitle("9xCode Interpreter");
                    BetterConsole.Title = "GTerm";
                    BetterConsole.Clear();
                    Kernel.DrawPrompt();
                    break;

                default:
                    WindowManager.AddWindow(new OpenWithFrame());
                    break;
            }
        }
    }
}
