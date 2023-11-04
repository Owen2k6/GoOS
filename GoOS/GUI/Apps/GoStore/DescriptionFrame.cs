﻿using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class DescriptionFrame : Window
    {
        public DescriptionFrame(string name, string version, string author, string description, string language)
        {
            // Generate the fonts.
            Generate(ResourceType.Fonts);

            // Create the window.
            Contents = new Canvas(300, 250);
            Title = "GoStore";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);

            // Paint the window.
            Contents.Clear(Color.LightGray);
            RenderSystemStyleBorder();
            Contents.DrawString(10, 10, name, Font_2x, Color.White);
            Contents.DrawString(10, 56, "Version: " + version.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 72, "Author: " + author.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 88, "Language: " + language.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 100, "Description: " + description.Replace("\\n", "\n"), Font_1x, Color.White);
        }

        public override void HandleRun()
        {
            base.HandleRun();
        }
    }
}
