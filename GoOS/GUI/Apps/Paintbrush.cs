using System;
using System.Collections.Generic;
using Cosmos.System;
using GoGL.Graphics;
using GoOS.GUI.Models;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public enum PaintTools
{
    Brush = 0,
    Bucket = 1,
    Text = 2,
    Rubber
}

public class Paintbrush : Window
{
    private Button AboutButton;

    private Color BackgroundColor = Color.White;

    private int BrushSize = 1;

    private Input Dialog_TextBox;

    private int OldX, OldY;

    private Color SelectedColor = Color.Black;

    private int TextX, TextY;

    private Button[] Utilities;

    private PaintTools Utility;

    private ushort Width, Height;

    public Paintbrush()
    {
        var sizeDialogue = new Dialogue(
            "Canvas size",
            "Please input canvas size:",
            new List<DialogueButton>
            {
                new()
                {
                    Text = "OK",
                    Callback = Size_Handler
                },
                new()
                {
                    Text = "Cancel"
                }
            },
            question);

        Dialog_TextBox = new Input(sizeDialogue, 80, 52, 195, 20, "800x600");

        WindowManager.AddWindow(sizeDialogue);
    }

    private bool IsOverColorTable => MouseManager.X > X + 54 && MouseManager.X < X + 278 &&
                                     MouseManager.Y > Y + Convert.ToUInt16(Contents.Height - 26) &&
                                     MouseManager.Y < Y + Convert.ToUInt16(Contents.Height + 6);

    private bool IsOverPaintableArea => MouseManager.X > X && MouseManager.X < X + Contents.Width - BrushSize &&
                                        MouseManager.Y > Y + TITLE_BAR_HEIGHT &&
                                        MouseManager.Y < Y + Convert.ToUInt16(Contents.Height - 36);

    private void Size_Handler()
    {
        try
        {
            if (Dialog_TextBox.Text == string.Empty)
            {
                Width = 800;
                Height = 600;
            }
            else
            {
                var dimensions = Dialog_TextBox.Text.Replace(" ", "").Split("x");
                Width = ushort.Parse(dimensions[0]);
                Height = ushort.Parse(dimensions[1]);
            }
        }
        catch (Exception)
        {
            Dialogue.Show("Error", "Invalid canvas size syntax!");
            Dispose();
            return;
        }

        Contents = new Canvas(Width, Height);
        Contents.Clear(BackgroundColor);
        Title = "Paintbrush";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        AboutButton =
            new Button(this, Convert.ToUInt16(Contents.Width - 36), Convert.ToUInt16(Contents.Height - 38), 24, 20, "?")
                { Clicked = AboutButton_Click };
        Utilities = new Button[4]
        {
            new(this, 12, Convert.ToUInt16(Contents.Height - 42), 16, 16, string.Empty)
            {
                Image = brush,
                Clicked = Pencil_Click
            },
            new(this, 28, Convert.ToUInt16(Contents.Height - 42), 16, 16, string.Empty)
            {
                Image = bucket,
                Clicked = Bucket_Click
            },
            new(this, 12, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty)
            {
                Image = text,
                Clicked = Text_Click
            },
            new(this, 28, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty)
            {
                Image = rubber,
                Clicked = Rubber_Click
            }
        };

        RenderPanel();
    }

    private void AboutButton_Click()
    {
        ShowAboutDialog("1.1");
    }

    public override void HandleKey(KeyEvent key)
    {
        switch (key.Key)
        {
            case ConsoleKeyEx.F1:
                ShowAboutDialog();
                break;
        }

        // TODO: implement ctrl + z
    }

    private void Pencil_Click()
    {
        Utility = PaintTools.Brush;

        var pencilDialogue = new Dialogue(
            "Brush size",
            "Please input brush size:",
            new List<DialogueButton>
            {
                new()
                {
                    Text = "OK",
                    Callback = Pencil_Handler
                },
                new()
                {
                    Text = "Cancel"
                }
            },
            question);

        Dialog_TextBox = new Input(pencilDialogue, 80, 52, 187, 20, BrushSize.ToString());

        WindowManager.AddWindow(pencilDialogue);
    }

    private void Pencil_Handler()
    {
        var value = Convert.ToInt32(Dialog_TextBox.Text.Trim());

        if (Dialog_TextBox.Text.Trim().Length == 0 || value < 1)
        {
            BrushSize = 1;
            throw new Exception("You must input a valid brush size");
        }

        BrushSize = value;
    }

    private void Bucket_Click()
    {
        Utility = PaintTools.Bucket;
    }

    private void Text_Click()
    {
        Utility = PaintTools.Text;
    }

    private void Rubber_Click()
    {
        Utility = PaintTools.Rubber;
    }

    private void Text_Handler()
    {
        Contents.DrawString(TextX, TextY, Dialog_TextBox.Text, Font_1x, SelectedColor);
    }

    public override void HandleRun()
    {
        if (!WindowManager.AreThereDraggingWindows && Focused && IsMouseOver && IsOverPaintableArea)
        {
            switch (Utility)
            {
                case PaintTools.Brush:
                    WindowManager.MouseToDraw = brush;
                    WindowManager.MouseOffsetX = 4;
                    WindowManager.MouseOffsetY = 14;
                    break;

                case PaintTools.Bucket:
                    WindowManager.MouseToDraw = bucket;
                    break;

                case PaintTools.Text:
                    WindowManager.MouseToDraw = mouse_text;
                    break;

                case PaintTools.Rubber:
                    WindowManager.MouseToDraw = rubber;
                    break;
            }

            if (MouseManager.LastMouseState == MouseState.None)
            {
                OldX = (int)MouseManager.X;
                OldY = (int)MouseManager.Y;
            }

            if (MouseManager.MouseState == MouseState.Left && (OldX != MouseManager.X || OldY != MouseManager.Y))
                switch (Utility)
                {
                    case PaintTools.Brush:
                        DrawLine(OldX - X - 1, OldY - Y - 19, (int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19,
                            (ushort)BrushSize, SelectedColor);
                        OldX = (int)MouseManager.X;
                        OldY = (int)MouseManager.Y;
                        break;

                    case PaintTools.Rubber:
                        DrawLine(OldX - X - 1, OldY - Y - 19, (int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19,
                            10, BackgroundColor);
                        OldX = (int)MouseManager.X;
                        OldY = (int)MouseManager.Y;
                        break;
                }
        }
    }

    public override void HandleClick(MouseEventArgs e)
    {
        if (IsOverPaintableArea)
            switch (Utility)
            {
                case PaintTools.Bucket:
                    Contents.DrawFilledRectangle(0, 0, Convert.ToUInt16(Contents.Width - 2),
                        Convert.ToUInt16(Contents.Height - 52), 0, SelectedColor);
                    BackgroundColor = SelectedColor;
                    break;

                case PaintTools.Text:
                    TextX = (int)MouseManager.X - X - 1;
                    TextY = (int)MouseManager.Y - Y - 1;

                    var pencilDialogue = new Dialogue(
                        "Label text",
                        "Please input label text:",
                        new List<DialogueButton>
                        {
                            new()
                            {
                                Text = "OK",
                                Callback = Text_Handler
                            },
                            new()
                            {
                                Text = "Cancel"
                            }
                        },
                        question);

                    Dialog_TextBox = new Input(pencilDialogue, 80, 52, 187, 20, string.Empty);

                    WindowManager.AddWindow(pencilDialogue);
                    break;
            }

        if (IsOverColorTable)
            SelectedColor = Contents[(int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19];

        RenderButtons();
    }

    private void RenderPanel()
    {
        Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 52), Convert.ToUInt16(Contents.Width - 4),
            50, 0, Color.DeepGray);
        Contents.DrawImage(54, Convert.ToUInt16(Contents.Height - 42), colorTable, false);
        RenderButtons();
        RenderSystemStyleBorder();
    }

    private void RenderButtons()
    {
        AboutButton.pressed = false;
        AboutButton.Render();

        foreach (var util in Utilities)
        {
            util.pressed = false;
            util.Render();
        }
    }

    /* Paint utilities */
    private void DrawPoint(int X, int Y, ushort size, Color color)
    {
        Contents.DrawFilledRectangle(X, Y, size, size, 0, color);
    }

    private void DrawLine(int X1, int Y1, int X2, int Y2, ushort size, Color color)
    {
        int DX = Math.Abs(X2 - X1), SX = X1 < X2 ? 1 : -1;
        int DY = Math.Abs(Y2 - Y1), SY = Y1 < Y2 ? 1 : -1;
        var err = (DX > DY ? DX : -DY) / 2;

        while (X1 != X2 || Y1 != Y2)
        {
            DrawPoint(X1, Y1, size, color);

            var E2 = err;

            if (E2 > -DX)
            {
                err -= DY;
                X1 += SX;
            }

            if (E2 < DY)
            {
                err += DX;
                Y1 += SY;
            }
        }
    }
}