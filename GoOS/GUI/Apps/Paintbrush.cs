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
        Bucket = 1
    }

    public class Paintbrush : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.colors.bmp")] private static byte[] colorTableRaw;
        private static Canvas colorTable = Image.FromBitmap(colorTableRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.brush.bmp")] private static byte[] brushRaw;
        private static Canvas brush = Image.FromBitmap(brushRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.bucket.bmp")] private static byte[] bucketRaw;
        private static Canvas bucket = Image.FromBitmap(bucketRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.question.bmp")] private static byte[] questionRaw;
        private static Canvas question = Image.FromBitmap(questionRaw, false);

        private Color SelectedColor = Color.Black;

        private Button AboutButton;

        private Button[] Utilities;

        private PaintTools Utility;

        private bool IsOverColorTable { get { return MouseManager.X > X + 54 && MouseManager.X < X + 278 && MouseManager.Y > Y + Convert.ToUInt16(Contents.Height - 26) && MouseManager.Y < Y + Convert.ToUInt16(Contents.Height + 6); } }

        private bool IsOverPaintableArea { get { return MouseManager.X > X && MouseManager.X < X + Contents.Width - BrushSize && MouseManager.Y > Y && MouseManager.Y < Y + Convert.ToUInt16(Contents.Height - 36); } }

        private int OldX, OldY;

        private int BrushSize = 1;

        private Dialogue PencilDialogue;

        private Input PencilDialoge_InputSize;

        public Paintbrush()
        {
            Contents = new Canvas(324, 300);
            Contents.Clear(Color.White);
            X = 50;
            Y = 50;
            Title = "Paintbrush";
            Visible = true;
            Closable = true;

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
                new Button(this, 12, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty),
                new Button(this, 28, Convert.ToUInt16(Contents.Height - 26), 16, 16, string.Empty),
            };

            RenderPanel();
        }

        public override void HandleKey(KeyEvent key)
        {
            if (key.Key == ConsoleKeyEx.F1)
                ShowAboutDialog();

            // TODO: implement ctrl + z
        }

        private void Pencil_Click()
        {
            Utility = PaintTools.Brush;

            PencilDialogue = new Dialogue(
                "Brush size",
                "Please enter a brush size:",
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

            PencilDialoge_InputSize = new Input(PencilDialogue, 80, 52, 232, 20, BrushSize.ToString());

            WindowManager.AddWindow(PencilDialogue);
        }

        private void Pencil_Handler()
        {
            int value = Convert.ToInt32(PencilDialoge_InputSize.Text.Trim());

            if (PencilDialoge_InputSize.Text.Trim().Length == 0 || value < 1)
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

        public override void HandleRun()
        {
            if (!WindowManager.AreThereDraggingWindows && Focused)
            {
                if (MouseManager.LastMouseState == MouseState.None)
                {
                    OldX = (int)MouseManager.X;
                    OldY = (int)MouseManager.Y;
                }

                if (MouseManager.MouseState == MouseState.Left && (OldX != MouseManager.X || OldY != MouseManager.Y) && IsOverPaintableArea && Utility == PaintTools.Brush)
                {
                    DrawLine(OldX - X - 1, OldY - Y - 19, (int)MouseManager.X - X - 1, (int)MouseManager.Y - Y - 19, (ushort)BrushSize);
                    OldX = (int)MouseManager.X;
                    OldY = (int)MouseManager.Y;
                }
            }
        }

        public override void HandleClick(MouseEventArgs e)
        {
            if (IsOverPaintableArea && Utility == PaintTools.Bucket)
                Contents.DrawFilledRectangle(0, 0, Convert.ToUInt16(Contents.Width - 2), Convert.ToUInt16(Contents.Height - 52), 0, SelectedColor);
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
