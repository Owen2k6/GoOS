using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoIDE
{
    public class ProjectsFrame : Window
    {
        Button[] RecentProjectsButtons;
        Button ImportButton;
        Button LoadExistingButton;
        Button CreateNewButton;

        public ProjectsFrame()
        {
            try
            {
                // Create the directories.
                if (!Directory.Exists(@"0:\content\prf")) Directory.CreateDirectory(@"0:\content\prf");
                if (!Directory.Exists(@"0:\content\prf\GoIDE")) Directory.CreateDirectory(@"0:\content\prf\GoIDE");
                if (!Directory.Exists(@"0:\content\prf\GoIDE\Projects")) Directory.CreateDirectory(@"0:\content\prf\GoIDE\Projects");
                if (!Directory.Exists(@"0:\content\prf\GoIDE\SaveData")) Directory.CreateDirectory(@"0:\content\prf\GoIDE\SaveData");

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

                ImportButton = new Button(this, Convert.ToUInt16(Contents.Width - 382), Convert.ToUInt16(Contents.Height - 30), 88, 20, "Import...") { Clicked = ImportButton_Click };
                LoadExistingButton = new Button(this, Convert.ToUInt16(Contents.Width - 284), Convert.ToUInt16(Contents.Height - 30), 144, 20, "Load existing...") { Clicked = LoadExistingButton_Click };
                CreateNewButton = new Button(this, Convert.ToUInt16(Contents.Width - 130), Convert.ToUInt16(Contents.Height - 30), 120, 20, "Create new...") { Clicked = CreateNewButton_Click };

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                Contents.DrawString(10, 10, "All projects", Fonts.Font_2x, Color.White);
                foreach (Button i in RecentProjectsButtons) i.Render();
                ImportButton.Render();
                LoadExistingButton.Render();
                CreateNewButton.Render();
            }
            catch (Exception e)
            {
                Dialogue.Show("TheTunaFishSandwitch is racist!!1", e.Message, null, WindowManager.errorIcon);
            }
        }

        void RecentProjects_Click(string i)
        {
            WindowManager.AddWindow(new IDEFrame(i.Remove(i.LastIndexOf(".")), @"0:\content\prf\GoIDE\Projects\" + i, i.EndsWith(".9xc")));
            Dispose();
        }

        void ImportButton_Click()
        {
            WindowManager.AddWindow(new ImportProjectFrame());
            Dispose();
        }

        void LoadExistingButton_Click()
        {
            WindowManager.AddWindow(new LoadProjectFrame());
            Dispose();
        }

        void CreateNewButton_Click()
        {
            WindowManager.AddWindow(new NewProjectFrame());
            Dispose();
        }
    }
}
