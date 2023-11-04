using System;
using System.IO;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoIDE
{
    public class ProjectsFrame : Window
    {
        Button[] RecentProjectsButtons;
        Button DeleteButton;
        Button ImportButton;
        Button LoadExistingButton;
        Button CreateNewButton;

        public ProjectsFrame()
        {
            try
            {
                // Create the directories.
                if (!Directory.Exists(@"0:\content\prf\GoIDE") || !Directory.Exists(@"0:\content\prf\GoIDE\Projects") || !Directory.Exists(@"0:\content\prf\GoIDE\SaveData"))
                {
                    WindowManager.AddWindow(new WelcomeFrame());
                    Dispose(); return;
                }

                // Generate the fonts.
                Fonts.Generate();

                // Create the window.
                Contents = new Canvas(400, 300);
                Title = "All projects - GoIDE";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                string[] recentProjects = Directory.GetFiles(@"0:\content\prf\GoIDE\Projects\");

                RecentProjectsButtons = new Button[recentProjects.Length];

                for (int i = 0; i < recentProjects.Length; i++)
                {
                    RecentProjectsButtons[i] = new Button(this, Convert.ToUInt16(10 + (i / 10 * 185)), Convert.ToUInt16(52 + ((i * 20) - (i / 10 * 200))), Convert.ToUInt16(recentProjects[i].Length * 8), 20, recentProjects[i])
                    {
                        Name = recentProjects[i],
                        UseSystemStyle = false,
                        BackgroundColour = Color.LightGray,
                        SelectionColour = new Color(100, 100, 100),
                        HasSelectionColour = true,
                        ClickedAlt = RecentProjects_Click
                    };
                }

                DeleteButton = new Button(this, Convert.ToUInt16(Contents.Width - 380), Convert.ToUInt16(Contents.Height - 30), 64, 20, "Delete") { Clicked = DeleteButton_Click };
                ImportButton = new Button(this, Convert.ToUInt16(Contents.Width - 306), Convert.ToUInt16(Contents.Height - 30), 64, 20, "Import") { Clicked = ImportButton_Click };
                LoadExistingButton = new Button(this, Convert.ToUInt16(Contents.Width - 234), Convert.ToUInt16(Contents.Height - 30), 120, 20, "Load existing") { Clicked = LoadExistingButton_Click };
                CreateNewButton = new Button(this, Convert.ToUInt16(Contents.Width - 106), Convert.ToUInt16(Contents.Height - 30), 96, 20, "Create new") { Clicked = CreateNewButton_Click };

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                Contents.DrawString(10, 10, "All projects", Fonts.Font_2x, Color.White);
                foreach (Button i in RecentProjectsButtons) i.Render();
                DeleteButton.Render();
                ImportButton.Render();
                LoadExistingButton.Render();
                CreateNewButton.Render();
            }
            catch { }
        }

        private void RecentProjects_Click(string i)
        {
            if (DeleteButton.AppearPressed)
            {
                File.Delete(@"0:\content\prf\GoIDE\Projects\" + i);
                Dispose();
                WindowManager.AddWindow(new ProjectsFrame());
            }
            else
            {
                WindowManager.AddWindow(new IDEFrame(i.Remove(i.LastIndexOf(".")), @"0:\content\prf\GoIDE\Projects\" + i, i.EndsWith(".9xc")));
                Dispose();
            }
        }

        private void ImportButton_Click()
        {
            WindowManager.AddWindow(new ImportProjectFrame());
            Dispose();
        }

        private void LoadExistingButton_Click()
        {
            WindowManager.AddWindow(new LoadProjectFrame());
            Dispose();
        }

        private void CreateNewButton_Click()
        {
            WindowManager.AddWindow(new NewProjectFrame());
            Dispose();
        }

        private void DeleteButton_Click()
        {
            DeleteButton.AppearPressed = !DeleteButton.AppearPressed;
        }
    }
}
