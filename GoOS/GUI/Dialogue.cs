using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI
{
    public struct DialogueButton
    {
        public string Text;
        public Action Callback;
    }

    public class Dialogue : Window
    {
        private const int buttonSpacing = 20;

        private const int buttonPadding = 20;

        /// <summary>
        /// Show a system dialogue.
        /// </summary>
        public static Dialogue Show(string title, string message, List<DialogueButton> buttons = null,
            Canvas icon = null, int widthOverride = -1, int heightOverride = -1)
        {
            // TODO: finish overrides

            var dialogue = new Dialogue(title, message, buttons, icon);
            WindowManager.AddWindow(dialogue);
            WindowManager.Update();
            return dialogue;
        }

        public static int GetLongestLineWidth(string str)
        {
            int len = 0;

            foreach (var line in str.Split('\n'))
            {
                len = Math.Max(
                    len,
                    Resources.Font_1x.MeasureString(line)
                );
            }

            return len;
        }

        public Dialogue(string title, string message, List<DialogueButton> buttons = null, Canvas icon = null)
        {
            if (icon == null)
            {
                // default icon
                icon = infoIcon;
            }

            if (buttons == null)
            {
                // Default buttons

                buttons = new()
                {
                    new() { Text = "OK" }
                };
            }

            Contents = new Canvas(
                Width: (ushort)(100 + GetLongestLineWidth(message)),
                Height: 128
            );
            RenderOutsetWindowBackground();
            SetDock(WindowDock.Center);
            Title = title;
            Visible = true;
            Closable = true;

            Contents.DrawImage(20, 20, icon, true);

            Contents.DrawString(80, 20, message, Resources.Font_1x, Color.White);

            int x = Contents.Width;
            foreach (DialogueButton dialogueButton in buttons)
            {
                ushort width = (ushort)(Resources.Font_1x.MeasureString(dialogueButton.Text) + (buttonPadding * 2));
                x -= width + buttonSpacing;

                Button button = new Button(
                    this,
                    (ushort)x,
                    (ushort)(Contents.Height - 25 - 20),
                    width,
                    25,
                    dialogueButton.Text
                );

                button.Clicked = () =>
                {
                    dialogueButton.Callback?.Invoke();

                    Dispose();
                };
            }

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }
    }
}