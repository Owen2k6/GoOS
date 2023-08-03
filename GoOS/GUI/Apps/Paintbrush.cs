using System;
using Cosmos.System;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public enum PaintTools
    {
        Brush = 0,
        Bucket = 1,
        Text = 2
    }

    public class Paintbrush : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.colors.bmp")] private static byte[] colorTableRaw;
        private static Canvas colorTable = Image.FromBitmap(colorTableRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.brush.bmp")] private static byte[] brushRaw;
        private static Canvas brush = Image.FromBitmap(brushRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.bucket.bmp")] private static byte[] bucketRaw;
        private static Canvas bucket = Image.FromBitmap(bucketRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.text.bmp")] private static byte[] textRaw;
        private static Canvas text = Image.FromBitmap(textRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.question.bmp")] private static byte[] questionRaw;
        private static Canvas question = Image.FromBitmap(questionRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse_text.bmp")] private static byte[] mouse_textRaw;
        private static Canvas mouse_text = Image.FromBitmap(mouse_textRaw, false);

        private bool IsOverColorTable { get { return MouseManager.X > X + 54 && MouseManager.X < X + 278 && MouseManager.Y > Y + Convert.ToUInt16(Contents.Height - 26) && MouseManager.Y < Y + Convert.ToUInt16(Contents.Height + 6); } }

        private bool IsOverPaintableArea { get { return MouseManager.X > X && MouseManager.X < X + Contents.Width - BrushSize && MouseManager.Y > Y + TITLE_BAR_HEIGHT && MouseManager.Y < Y + Convert.ToUInt16(Contents.Height - 36); } }

        private Color SelectedColor = Color.Black;

        private Button AboutButton;

        private Button[] Utilities;

        private Input Dialog_TextBox;

        private PaintTools Utility;

        private int OldX, OldY;

        private int BrushSize = 1;

        private int TextX, TextY;

        public Paintbrush()
        {
            Contents = new Canvas(324, 300);
            Contents.Clear(Color.LightGray);
            Title = "Paintbrush";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            AboutButton = new Button(this, Convert.ToUInt16(Contents.Width - 36), Convert.ToUInt16(Contents.Height - 38), 24, 20, "?") { Clicked = ShowAboutDialog };
            Utilities = new Button[4]
            {
                new Button(this, 12, Convert.ToUInt16(Contents.Height - 42), 16, 16, string.Empty)
                {
                    Image = brush,
                    Clicked = Pencil_Click
                },
                new Button(this, 28, Convert.ToUInt16(Contents.Height - 42), 16, 16, string.Empty)
                {
                    Image = bucket,
                    Clicked = Bucket_Click
                },
                new Button(this, 12, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty)
                {
                    Image = text,
                    Clicked = Text_Click
                },
                new Button(this, 28, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty),
            };

            RenderPanel();
        }

        public override void HandleKey(KeyEvent key)
        {
            if (key.Key == ConsoleKeyEx.F1)
                ShowAboutDialog();

            // TODO: implement ctrl + z
            // What about a fucking eraser? - Owen2k6
        }

        private void Pencil_Click()
        {
            Utility = PaintTools.Brush;

            Dialogue pencilDialogue = new Dialogue(
                "Brush size",
                "Please input brush size:",
                new System.Collections.Generic.List<DialogueButton>()
                {
                    new DialogueButton()
                    {
                        Text = "OK",
                        Callback = Pencil_Handler
                    },
                    new DialogueButton()
                    {
                        Text = "Cancel",
                    }
                },
                question);

            Dialog_TextBox = new Input(pencilDialogue, 80, 52, 232, 20, BrushSize.ToString());

            WindowManager.AddWindow(pencilDialogue);
        }

        private void Pencil_Handler()
        {
            int value = Convert.ToInt32(Dialog_TextBox.Text.Trim());

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

        private void Text_Handler()
        {
            Contents.DrawString(TextX, TextY, Dialog_TextBox.Text, BetterConsole.font, SelectedColor);
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
                }

                if (MouseManager.LastMouseState == MouseState.None)
                {
                    OldX = (int)MouseManager.X;
                    OldY = (int)MouseManager.Y;
                }

                if (MouseManager.MouseState == MouseState.Left && (OldX != MouseManager.X || OldY != MouseManager.Y))
                {
                    if (Utility == PaintTools.Brush)
                    {
                        DrawLine(OldX - X - 1, OldY - Y - 19, (int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19, (ushort)BrushSize);
                        OldX = (int)MouseManager.X;
                        OldY = (int)MouseManager.Y;
                    }
                }
            }
        }

        public override void HandleClick(MouseEventArgs e)
        {
            if (IsOverPaintableArea)
            {
                switch (Utility)
                {
                    case PaintTools.Bucket:
                        Contents.DrawFilledRectangle(0, 0, Convert.ToUInt16(Contents.Width - 2), Convert.ToUInt16(Contents.Height - 52), 0, SelectedColor);
                        break;

                    case PaintTools.Text:
                        TextX = (int)MouseManager.X - X - 1;
                        TextY = (int)MouseManager.Y - Y - 1;

                        Dialogue pencilDialogue = new Dialogue(
                            "Label text",
                            "Please input label text:",
                            new System.Collections.Generic.List<DialogueButton>()
                            {
                                new DialogueButton()
                                {
                                    Text = "OK",
                                    Callback = Text_Handler
                                },
                                new DialogueButton()
                                {
                                    Text = "Cancel",
                                }
                            },
                            question);

                        Dialog_TextBox = new Input(pencilDialogue, 80, 52, 232, 20, string.Empty);

                        WindowManager.AddWindow(pencilDialogue);
                        break;
                }
            }
                
            if (IsOverColorTable)
                SelectedColor = Contents[(int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19];

            RenderButtons();
        }

        private void RenderPanel()
        {
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 52), Convert.ToUInt16(Contents.Width - 4), 50, 0, new Color(234, 234, 234));
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
        private void DrawPoint(int X, int Y, ushort size)
        {
            Contents.DrawFilledRectangle(X, Y, size, size, 0, SelectedColor);
        }

        private void DrawLine(int X1, int Y1, int X2, int Y2, ushort size)
        {
            int DX = Math.Abs(X2 - X1), SX = X1 < X2 ? 1 : -1;
            int DY = Math.Abs(Y2 - Y1), SY = Y1 < Y2 ? 1 : -1;
            int err = (DX > DY ? DX : -DY) / 2;

            while (X1 != X2 || Y1 != Y2)
            {
                DrawPoint(X1, Y1, size);

                int E2 = err;

                if (E2 > -DX) { err -= DY; X1 += SX; }
                if (E2 < DY) { err += DX; Y1 += SY; }
            }
        }
    }
}
