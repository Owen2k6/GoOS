using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cosmos.System;
using GoOS.Commands;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.Gosplorer
{
    public class MainFrame : Window
    {
        const int IconWidth = 64, IconHeight = 80;
        string Path = @"0:\";

        List Dialog_List;
        Input AddressBar;
        Input Dialog_TextBox;
        Button BackButton;
        Button ForwardButton;
        Button UpButton;
        Button RefreshButton;
        Button ContextButton;

        Button[] Shortcuts;
        Button[] FolderContents;

        List<string> BrowseHistory;
        int BrowseHistoryIndex;

        public MainFrame()
        {
            Contents = new Canvas(835, 600);
            Title = Path + " - Gosplorer";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            AddressBar = new Input(this, 90, 10, (ushort)(Contents.Width - 100 - 20 - 10), 20, "Path") { Text = Path, Submitted = AddressBar_Submit };
            BackButton = new Button(this, 3, 6, 26, 26, string.Empty)
            {
                Image = arrowleft,
                Clicked = BackButton_Click,
                UseSystemStyle = false,
                RenderWithAlpha = true,
                BackgroundColour = Color.Transparent
            };
            ForwardButton = new Button(this, 29, 6, 26, 26, string.Empty)
            {
                Image = arrowright,
                Clicked = ForwardButton_Click,
                UseSystemStyle = false,
                RenderWithAlpha = true,
                BackgroundColour = Color.Transparent
            };
            UpButton = new Button(this, 59, 6, 26, 26, string.Empty)
            {
                Image = arrowup,
                Clicked = UpArrow_Click,
                UseSystemStyle = false,
                RenderWithAlpha = true,
                BackgroundColour = Color.Transparent
            };
            RefreshButton = new Button(this, 800, 6, 26, 26, string.Empty)
            {
                Image = refIcon,
                Clicked = RenderFolderItems,
                UseSystemStyle = false,
                RenderWithAlpha = true,
                BackgroundColour = Color.Transparent
            };

            Shortcuts = new Button[]
            {
                new Button(this, 26, 45, 40, 20, @"0:\")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                    Name = @"0:\",
                    ClickedAlt = Shortcut_Click
                },
                new Button(this, 26, 70, 40, 20, @"1:\")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                    Name = @"1:\",
                    ClickedAlt = Shortcut_Click
                },
                new Button(this, 26, 95, 48, 20, "Apps")
                {
                    UseSystemStyle = false,
                    RenderWithAlpha = true,
                    BackgroundColour = new Color(0, 0, 0, 0),
                    Name = "Apps",
                    ClickedAlt = Shortcut_Click
                }
            };

            BrowseHistory = new List<string> { @"0:\" };

            RenderFolderItems();
        }

        public override void Paint()
        {
            Contents.DrawImage(0, 0, appbackground, false);
            Contents.DrawImage(0, 0, header, false);
            Contents.DrawImage(0, 40, sidebar, false);
            Contents.DrawImage(10, 45, drive);
            Contents.DrawImage(10, 70, drive_locked);
            Contents.DrawImage(10, 95, ideIconSmall);

            AddressBar.Render();
            BackButton.Render();
            ForwardButton.Render();
            UpButton.Render();
            RefreshButton.Render();

            foreach (Button i in Shortcuts) i.Render();
            foreach (Button i in FolderContents) if (i != null) i.Render();

            RenderSystemStyleBorder();
        }

        private Button GetButtonUnderMouse()
        {
            foreach (Button i in FolderContents) if (i.IsMouseOver) return i;
            return null;
        }

        private bool IsMouseOverFolderArea
        {
            get
            {
                return MouseManager.X >= X + 84 &&
                       MouseManager.X < X + 84 + Contents.Width &&
                       MouseManager.Y >= Y + 40 &&
                       MouseManager.Y < Y + 40 + Contents.Height;
            }
        }

        public override void ShowContextMenu()
        {
            string[] contextMenuEntries = Array.Empty<string>();
            ContextButton = GetButtonUnderMouse();

            if (!IsMouseOverFolderArea) return;
            else if (ContextButton != null && ContextButton.Image == folderIcon) contextMenuEntries = new[] { " Open", " Delete" };
            else if (ContextButton != null && ContextButton.Image == fileIcon) contextMenuEntries = new[] { " Open", " Delete", string.Empty, " Pin to start menu", " Open with..." };
            //else if (ContextButton != null && ContextButton.Image == fileIcon && ContextButton.Name.EndsWith(".goexe") || ContextButton.Name.EndsWith(".gexe") || ContextButton.Name.EndsWith(".9xc")) contextMenuEntries = new[] { " Open", " Delete", string.Empty, " Pin to start menu", " Open with..." };
            else if (!Path.StartsWith(@"1:\")) contextMenuEntries = new[] { " New Folder", " New File" };

            ContextMenu.Show(contextMenuEntries.ToArray(), (ContextButton.Image == fileIcon || ContextButton.Image == folderIcon) ? (ushort)64 : (ushort)96, ContextMenu_Handle);
        }

        private void ContextMenu_Handle(string item)
        {
            switch (item)
            {
                case " Open":
                    FolderContents_Clicked(ContextButton.Name);
                    break;
                
                case " Delete":
                    if (Directory.Exists(Path + @"\" + ContextButton.Name)) Directory.Delete(Path + @"\" + ContextButton.Name, true);
                    else File.Delete(Path + @"\" + ContextButton.Name);

                    RenderFolderItems();
                    break;

                case " Pin to start menu":
                    List<string> lines = new List<string>(File.ReadAllLines(@"0:\content\sys\pinnedapps.gms")) { Path + @"\" + ContextButton.Name };
                    File.WriteAllLines(@"0:\content\sys\pinnedapps.gms", lines.ToArray());
                    break;

                case " Open with...":
                    Dialogue openWithDialogue = new Dialogue(
                        "Open with...",
                        "Select the app to open with:",
                        new List<DialogueButton>()
                        {
                            new DialogueButton()
                            {
                                Text = "OK",
                                Callback = NewFolder_Handler
                            },
                            new DialogueButton()
                            {
                                Text = "Cancel"
                            }
                        },
                        question);

                    Dialog_List = new List(openWithDialogue, 80, 52, 195, 20, "Applications", new string[] { "Notepad", "GoCode Interpreter", "9xCode Interpreter", "GoIDE", "Gimviewer" });

                    WindowManager.AddWindow(openWithDialogue);
                    break;

                case " New Folder":
                    Dialogue folderDialogue = new Dialogue(
                        "New Folder",
                        "Please input folder name:",
                        new List<DialogueButton>()
                        {
                            new DialogueButton()
                            {
                                Text = "OK",
                                Callback = NewFolder_Handler
                            },
                            new DialogueButton()
                            {
                                Text = "Cancel"
                            }
                        },
                        question);

                    Dialog_TextBox = new Input(folderDialogue, 80, 52, 195, 20, "Folder name");

                    WindowManager.AddWindow(folderDialogue);
                    break;

                case " New File":
                    Dialogue fileDialogue = new Dialogue(
                        "New Folder",
                        "Please input filename:",
                        new List<DialogueButton>()
                        {
                            new DialogueButton()
                            {
                                Text = "OK",
                                Callback = NewFile_Handler
                            },
                            new DialogueButton()
                            {
                                Text = "Cancel"
                            }
                        },
                        question);

                    Dialog_TextBox = new Input(fileDialogue, 80, 52, 170, 20, "Filename");

                    WindowManager.AddWindow(fileDialogue);
                    break;
            }
        }
        
        private void OpenWith_Handler()
        {
            Dialogue.Show("Debug", "This function is not implemented yet");
        }

        private void NewFolder_Handler()
        {
            if (Dialog_TextBox.Text.Trim().Length == 0)
            {
                Dialogue.Show("Error", "Folder name cannot be empty!", null, WindowManager.errorIcon);
                return;
            }

            Directory.CreateDirectory(Path + (Path.EndsWith(@"\") ? "" : @"\") + Dialog_TextBox.Text);
            RenderFolderItems();
        }

        private void NewFile_Handler()
        {
            if (Dialog_TextBox.Text.Trim().Length == 0)
            {
                Dialogue.Show("Error", "Folder name cannot be empty!", null, WindowManager.errorIcon);
                return;
            }

            File.Create(Path + (Path.EndsWith(@"\") ? "" : @"\") + Dialog_TextBox.Text);
            RenderFolderItems();
        }

        private void AddressBar_Submit()
        {
            BrowseHistory.Add(AddressBar.Text);
            BrowseHistoryIndex++;
            RenderFolderItems();
        }

        private void BackButton_Click()
        {
            if (BrowseHistory.Count == 0 || BrowseHistoryIndex == 0) return;
            if (BrowseHistoryIndex < 0) BrowseHistoryIndex = 0;

            BrowseHistoryIndex--;
            Path = BrowseHistory[BrowseHistoryIndex];
            AddressBar.Text = Path;
            RenderFolderItems();
        }

        private void ForwardButton_Click()
        {
            if (BrowseHistory.Count == 0 || BrowseHistoryIndex == BrowseHistory.Count - 1) return;
            if (BrowseHistoryIndex > BrowseHistory.Count - 1) BrowseHistoryIndex = BrowseHistory.Count - 1;

            BrowseHistoryIndex++;
            Path = BrowseHistory[BrowseHistoryIndex];
            AddressBar.Text = Path;
            RenderFolderItems();
        }

        private void UpArrow_Click()
        {
            Path = Path.Remove(Path.LastIndexOf("\\"));
            AddressBar.Text = Path;

            BrowseHistory.Add(AddressBar.Text);
            BrowseHistoryIndex++;
            RenderFolderItems();
        }

        private void Shortcut_Click(string e)
        {
            switch (e)
            {
                case { } a when a == @"0:\" || a == @"1:\":
                    Path = e;
                    AddressBar.Text = Path;
                    break;

                case "Apps":
                    Path = @"0:\go";
                    AddressBar.Text = Path;
                    break;
            }

            BrowseHistory.Add(AddressBar.Text);
            BrowseHistoryIndex++;
            RenderFolderItems();
        }

        private void RenderFolderItems()
        {
            string[] itemNames = Directory.GetDirectories(Path).Concat(Directory.GetFiles(Path)).ToArray();
            bool[] itemTypes = itemNames
                .Select(item => Directory.Exists(Path + (Path.EndsWith(@"\") ? "" : @"\") + item)).ToArray();
            int row = 0, column = 0;

            foreach (Button i in FolderContents) Controls.Remove(i);
            FolderContents = new Button[itemNames.Length];
            for (int i = 0; i < itemNames.Length; i++)
            {
                if (itemNames[i].EndsWith(".gms") && !Kernel.devMode) continue;

                if (column >= 10)
                {
                    column = 0;
                    row++;
                }

                FolderContents[i] = new Button(this, (ushort)(94 + (column * (IconWidth + 10))),
                    (ushort)(50 + (row * (IconHeight + 10))), IconWidth, IconHeight, itemNames[i])
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

            Title = Path + " - Gosplorer";

            Paint();
        }

        private void FolderContents_Clicked(string e)
        {
            if (Directory.Exists(Path + (Path.EndsWith(@"\") ? "" : @"\") + e))
            {
                Path += (Path.EndsWith(@"\") ? "" : @"\") + e;
                AddressBar.Text = Path;
                AddressBar_Submit();
            }
            else OpenAppByExtension(e);
        }

        private void OpenAppByExtension(string e)
        {
            switch (Path + (Path.EndsWith(@"\") ? "" : @"\") + e.ToLower())
            {
                case { } a when a.EndsWith(".txt") || a.EndsWith(".log") || a.EndsWith(".md") ||
                                    a.EndsWith(".gtheme"):
                    WindowManager.AddWindow(new Notepad(true, Path + (Path.EndsWith(@"\") ? "" : @"\") + e));
                    break;

                case { } a when a.EndsWith(".gexe") || a.EndsWith(".goexe"):
                    BetterConsole.Clear();
                    BetterConsole.Title = "GoCode Interpreter";
                    WindowManager.AddWindow(new GTerm(false));

                    Run.Main(Path + (Path.EndsWith(@"\") ? "" : @"\") + e, false);

                    WindowManager.RemoveWindowByTitle("GoCode Interpreter");
                    BetterConsole.Title = "GTerm";
                    BetterConsole.Clear();
                    Kernel.DrawPrompt();
                    break;

                case { } a when a.EndsWith(".9xc"):
                    BetterConsole.Clear();
                    BetterConsole.Title = "9xCode Interpreter";
                    WindowManager.AddWindow(new GTerm(false));

                    _9xCode.Interpreter.Run(Path + (Path.EndsWith(@"\") ? "" : @"\") + e);

                    WindowManager.RemoveWindowByTitle("9xCode Interpreter");
                    BetterConsole.Title = "GTerm";
                    BetterConsole.Clear();
                    Kernel.DrawPrompt();
                    break;

                case { } a when a.EndsWith(".bmp"):
                    WindowManager.AddWindow(
                        new Gimviewer(File.ReadAllBytes(Path + (Path.EndsWith(@"\") ? "" : @"\") + e), 0));
                    break;

                case { } a when a.EndsWith(".png"):
                    WindowManager.AddWindow(
                        new Gimviewer(File.ReadAllBytes(Path + (Path.EndsWith(@"\") ? "" : @"\") + e), 1));
                    break;

                case { } a when a.EndsWith(".ppm"):
                    WindowManager.AddWindow(
                        new Gimviewer(File.ReadAllBytes(Path + (Path.EndsWith(@"\") ? "" : @"\") + e), 2));
                    break;

                case { } a when a.EndsWith(".tga"):
                    WindowManager.AddWindow(
                        new Gimviewer(File.ReadAllBytes(Path + (Path.EndsWith(@"\") ? "" : @"\") + e), 3));
                    break;

                default:
                    Dialogue.Show("Error", "Unknown file extension!", null, WindowManager.errorIcon);
                    break;
            }
        }
    }
}