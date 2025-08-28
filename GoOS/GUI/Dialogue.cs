using System;
using System.Collections.Generic;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI;

public struct DialogueButton
{
    public string Text;
    public Action Callback;
}

public class Dialogue : Window
{
    private const int buttonSpacing = 20;
    private const int buttonPadding = 20;

    public Dialogue(string title, string message, List<DialogueButton> buttons = null, Canvas icon = null)
    {
        if (icon == null)
            // default icon
            icon = infoIcon;

        if (buttons == null)
            // Default buttons
            buttons = new List<DialogueButton>
            {
                new() { Text = "OK" }
            };

        Contents = new Canvas(
            (ushort)(100 + GetLongestLineWidth(message)),
            128
        );
        RenderOutsetWindowBackground();
        SetDock(WindowDock.Center);
        Title = title;
        Visible = true;
        Closable = true;

        Contents.DrawImage(20, 20, icon);

        Contents.DrawString(80, 20, message, Charcoal, Color.Black);

        int x = Contents.Width;
        foreach (var dialogueButton in buttons)
        {
            var width = (ushort)(Charcoal.MeasureString(dialogueButton.Text) + buttonPadding * 2);
            x -= width + buttonSpacing;

            var button = new Button(
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

        foreach (var control in Controls) control.Render();
    }

    /// <summary>
    ///     Show a system dialogue.
    /// </summary>
    public static Dialogue Show(string title, string message, List<DialogueButton> buttons = null,
        Canvas icon = null, int widthOverride = -1, int heightOverride = -1)
    {
        // TODO: finish overrides

        Kernel.InfoSound();
        var dialogue = new Dialogue(title, message, buttons, icon);
        WindowManager.AddWindow(dialogue);
        WindowManager.Update();
        return dialogue;
    }

    public static int GetLongestLineWidth(string str)
    {
        var len = 0;

        foreach (var line in str.Split('\n'))
            len = Math.Max(
                len,
                Charcoal.MeasureString(line)
            );

        return len;
    }
}