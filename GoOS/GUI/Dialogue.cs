using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public struct DialogueButton
    {
        public string Text;
        public Action Callback;
    }

    public class Dialogue : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.info.bmp")] private static byte[] infoIconRaw;
        private static Canvas infoIcon = Image.FromBitmap(infoIconRaw, false);

        private const int buttonSpacing = 20;

        private const int buttonPadding = 20;

        /// <summary>
        /// Show a system dialogue.
        /// </summary>
        public static Dialogue Show(string title, string message, List<DialogueButton> buttons = null, Canvas icon = null, int widthOverride = -1, int heightOverride = -1)
        {
            // TODO: finish overrides

            var dialogue = new Dialogue(title, message, buttons, icon);
            WindowManager.AddWindow(dialogue);
            return dialogue;
        }

        private static int GetLongestLineWidth(string str)
        {
            int len = 0;

            foreach (var line in str.Split('\n'))
            {
                len = Math.Max(
                    len,
                    BetterConsole.font.MeasureString(line)
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
            X = 480;
            Y = 296;
            Title = title;
            Visible = true;
            Closable = true;

            Contents.DrawImage(20, 20, icon, true);

            Contents.DrawString(80, 20, message, BetterConsole.font, Color.White);

            int x = Contents.Width;
            foreach (DialogueButton dialogueButton in buttons)
            {
                ushort width = (ushort)(BetterConsole.font.MeasureString(dialogueButton.Text) + (buttonPadding * 2));
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
