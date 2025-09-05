using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Cosmos.System;
using Gold.Graphics;
using GoOS._9xCode;
using GoOS.Apps;
using GoOS.Commands;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.Gosplorer;

public class MainFrame : Window
{
    private const int IconWidth = 64, IconHeight = 80;
    private readonly Input AddressBar;
    private readonly Button BackButton;

    private readonly List<string> BrowseHistory;
    private readonly Button ForwardButton;
    private readonly Button RefreshButton;
    private readonly Button ShowHiddenButton;

    private readonly Button[] Shortcuts;
    private readonly Button UpButton;
    private int BrowseHistoryIndex;
    private Button ContextButton;

    private List Dialog_List;
    private Input Dialog_TextBox;
    private Button[] FolderContents;
    private string Path = @"0:\";

    private bool ShowHidden = false;

    public MainFrame()
    {
        Contents = new Canvas(835, 600);
        Title = Path + " - Gosplorer";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        AddressBar = new Input(this, 90, 10, (ushort)(Contents.Width - 100 - 20 - 10), 20, "Path")
            { Text = Path, Submitted = AddressBar_Submit };
        BackButton = new Button(this, 3, 6, 26, 26, string.Empty)
        {
            Image = arrowleft,
            Clicked = BackButton_Click,
            UseSystemStyle = false,
            BackgroundColour = new Color(0, 0, 0)
        };
        ForwardButton = new Button(this, 29, 6, 26, 26, string.Empty)
        {
            Image = arrowright,
            Clicked = ForwardButton_Click,
            UseSystemStyle = false,
            BackgroundColour = new Color(0, 0, 0)
        };
        UpButton = new Button(this, 59, 6, 26, 26, string.Empty)
        {
            Image = arrowup,
            Clicked = UpArrow_Click,
            UseSystemStyle = false,
            BackgroundColour = new Color(0, 0, 0)
        };
        RefreshButton = new Button(this, 800, 6, 26, 26, string.Empty)
        {
            Image = refIcon,
            Clicked = RenderFolderItems,
            UseSystemStyle = false,
            BackgroundColour = new Color(0, 0, 0)
        };
        ShowHiddenButton = new Button(this, (ushort)(Contents.Width - 120), (ushort)(Contents.Height - 30), 110, 20, "Show Hidden")
        {
            UseSystemStyle = true,
            BackgroundColour = new Color(192, 192, 192),
            TextColour = Color.Black,
            Name = "HiddenToggle",
            ClickedAlt = HiddenToggle_Click
        };

        Shortcuts = new[]
        {
            new Button(this, 26, 45, 40, 20, @"0:\")
            {
                UseSystemStyle = false,
                BackgroundColour = new Color(192, 192, 192),
                TextColour = Color.Black,
                Name = @"0:\",
                ClickedAlt = Shortcut_Click
            },
            new Button(this, 26, 70, 40, 20, @"1:\")
            {
                UseSystemStyle = false,
                BackgroundColour = new Color(192, 192, 192),
                TextColour = Color.Black,
                Name = @"1:\",
                ClickedAlt = Shortcut_Click
            },
            new Button(this, 26, 95, 48, 20, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = new Color(192, 192, 192),
                TextColour = Color.Black,
                Name = "Apps",
                ClickedAlt = Shortcut_Click
            }
        };

        BrowseHistory = new List<string> { @"0:\" };

        RenderFolderItems();
    }

    private void HiddenToggle_Click(string _)
    {
        ShowHidden = !ShowHidden;
        ShowHiddenButton.AppearPressed = !ShowHiddenButton.AppearPressed;
        RenderFolderItems();
    }

    private bool IsMouseOverFolderArea =>
        MouseManager.X >= X + 84 &&
        MouseManager.X < X + 84 + Contents.Width &&
        MouseManager.Y >= Y + 40 &&
        MouseManager.Y < Y + 40 + Contents.Height;

    public override void Paint()
    {
        Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, new Color(0xFFF8F8F8));
        Contents.DrawFilledRectangle(0, 0, Contents.Width, 40, 0, new Color(0xFFE0E0E0));
        Contents.DrawFilledRectangle(0, 40, 84, Contents.Height, 0, new Color(0xFFC0C0C0));
        Contents.DrawLine(84, 40, 84, Contents.Height, Color.Black);
        Contents.DrawLine(0, 40, Contents.Width, 40, Color.Black);
        Contents.DrawImage(10, 45, drive);
        Contents.DrawImage(10, 70, drive_locked);
        Contents.DrawImage(10, 95, ideIconSmall);
        AddressBar.Render();
        BackButton.Render();
        ForwardButton.Render();
        UpButton.Render();
        RefreshButton.Render();
        ShowHiddenButton.Render();
        foreach (var i in Shortcuts) i.Render();
        if (FolderContents != null)
            foreach (var i in FolderContents)
                i?.Render();
        RenderSystemStyleBorder();
    }


    private Button GetButtonUnderMouse()
    {
        foreach (var i in FolderContents)
            if (i.IsMouseOver)
                return i;
        return null;
    }

    public override void ShowContextMenu()
    {
        var contextMenuEntries = Array.Empty<string>();
        ContextButton = GetButtonUnderMouse();

        if (!IsMouseOverFolderArea) return;

        var hasButton = ContextButton != null;
        var isFolder = hasButton && ContextButton.Image == folderIcon;
        var isFile = hasButton && ContextButton.Image == fileIcon;
        var name = hasButton ? ContextButton.Name : string.Empty;
        var ext = hasButton
            ? name.LastIndexOf('.') >= 0 ? name.Substring(name.LastIndexOf('.')).ToLower() : string.Empty
            : string.Empty;

        if (isFolder)
        {
            contextMenuEntries = new[] { " Open", " Delete", "----", " About Gosplorer" };
        }
        else if (isFile)
        {
            contextMenuEntries = new[] { " Open", " Edit", " Delete", "----", " About Gosplorer" };
        }
        else if (!Path.StartsWith(@"1:\"))
        {
            contextMenuEntries = new[] { " New Folder", " New File", "----", " About Gosplorer" };
        }

        ContextMenu.Show(contextMenuEntries.ToArray(), 136, ContextMenu_Handle);
    }

    private void ContextMenu_Handle(string item)
    {
        switch (item)
        {
            case " About Gosplorer":
                ShowAboutDialog("2.0");
                break;

            case " Open":
                FolderContents_Clicked(ContextButton.Name);
                break;

            case " Delete":
                if (Directory.Exists(Path + @"\" + ContextButton.Name))
                    Directory.Delete(Path + @"\" + ContextButton.Name, true);
                else File.Delete(Path + @"\" + ContextButton.Name);
                RenderFolderItems();
                break;

            case " Edit":
            {
                var full = Path + (Path.EndsWith(@"\") ? "" : @"\") + ContextButton.Name;
                WindowManager.AddWindow(new Notepad(true, full));
                break;
            }

            case " New Folder":
            {
                var folderDialogue = new Dialogue(
                    "New Folder",
                    "Please input folder name:",
                    new List<DialogueButton>
                    {
                        new() { Text = "OK", Callback = NewFolder_Handler },
                        new() { Text = "Cancel" }
                    },
                    question);

                Dialog_TextBox = new Input(folderDialogue, 80, 52, 195, 20, "Folder name");
                WindowManager.AddWindow(folderDialogue);
                break;
            }

            case " New File":
            {
                var fileDialogue = new Dialogue(
                    "New Folder",
                    "Please input filename:",
                    new List<DialogueButton>
                    {
                        new() { Text = "OK", Callback = NewFile_Handler },
                        new() { Text = "Cancel" }
                    },
                    question);

                Dialog_TextBox = new Input(fileDialogue, 80, 52, 170, 20, "Filename");
                WindowManager.AddWindow(fileDialogue);
                break;
            }
        }
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
        var itemNames = Directory.GetDirectories(Path).Concat(Directory.GetFiles(Path)).ToArray();
        var itemTypes = itemNames.Select(item => Directory.Exists(Path + (Path.EndsWith(@"\") ? "" : @"\") + item))
            .ToArray();
        int row = 0, column = 0;

        if (FolderContents != null)
            foreach (var i in FolderContents)
                Controls.Remove(i);

        FolderContents = new Button[itemNames.Length];
        for (var i = 0; i < itemNames.Length; i++)
        {
            if (itemNames[i].EndsWith(".gms") && !ShowHidden) continue;

            if (column >= 10)
            {
                column = 0;
                row++;
            }

            FolderContents[i] = new Button(this,
                (ushort)(94 + column * (IconWidth + 10)),
                (ushort)(50 + row * (IconHeight + 10)),
                IconWidth, IconHeight, itemNames[i])
            {
                UseSystemStyle = false,
                BackgroundColour = new Color(248, 248, 248),
                TextColour = Color.Black,
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
        else
        {
            OpenAppByExtension(e);
        }
    }

    private void OpenAppByExtension(string e)
    {
        var full = Path + (Path.EndsWith(@"\") ? "" : @"\") + e;
        var fullLower = full.ToLower();

        switch (fullLower)
        {
            case { } a when a.EndsWith(".txt") || a.EndsWith(".log") || a.EndsWith(".md") || a.EndsWith(".gtheme"):
                WindowManager.AddWindow(new Notepad(true, full));
                break;

            case { } a when a.EndsWith(".gexe") || a.EndsWith(".goexe"):
                BetterConsole.Clear();
                BetterConsole.Title = "GoCode Interpreter";
                WindowManager.AddWindow(new GTerm(false));
                Run.Main(full, false);
                WindowManager.RemoveWindowByTitle("GoCode Interpreter");
                BetterConsole.Title = "GTerm";
                BetterConsole.Clear();
                Kernel.DrawPrompt();
                break;

            case { } a when a.EndsWith(".9xc"):
                BetterConsole.Clear();
                BetterConsole.Title = "9xCode Interpreter";
                WindowManager.AddWindow(new GTerm(false));
                Interpreter.Run(full);
                WindowManager.RemoveWindowByTitle("9xCode Interpreter");
                BetterConsole.Title = "GTerm";
                BetterConsole.Clear();
                Kernel.DrawPrompt();
                break;

            // ---- NEW: GIFF scripts ----
            case { } a when a.EndsWith(".giff"):
                GiffRunner.RunFile(full);
                break;

            case { } a when a.EndsWith(".bmp"):
                WindowManager.AddWindow(new Gimviewer(File.ReadAllBytes(full), 0));
                break;

            case { } a when a.EndsWith(".png"):
                WindowManager.AddWindow(new Gimviewer(File.ReadAllBytes(full), 1));
                break;

            case { } a when a.EndsWith(".ppm"):
                WindowManager.AddWindow(new Gimviewer(File.ReadAllBytes(full), 2));
                break;

            case { } a when a.EndsWith(".tga"):
                WindowManager.AddWindow(new Gimviewer(File.ReadAllBytes(full), 3));
                break;

            default:
                Dialogue.Show("Error", "Unknown file extension!", null, WindowManager.errorIcon);
                break;
        }
    }
}