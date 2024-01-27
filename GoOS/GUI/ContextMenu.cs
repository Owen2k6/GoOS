using System;
using Cosmos.System;
using GoOS.GUI.Models;
using GoGL.Graphics;

namespace GoOS.GUI
{
    public class ContextMenu : Window
    {
        public string[] Items;

        public Action<string> Handle;

        public static ContextMenu Show(string[] items, ushort Width, Action<string> handle)
        {
            var contextmenu = new ContextMenu(items, Width) { Handle = handle };
            WindowManager.RemoveWindowByTitle(nameof(ContextMenu));
            WindowManager.AddWindow(contextmenu);
            return contextmenu;
        }

        public ContextMenu(string[] items, ushort Width)
        {
            Items = items;
            WindowManager.MouseMove = MouseMove;

            Contents = new Canvas(Width, Convert.ToUInt16(items.Length * 16 + 2));
            X = (int)MouseManager.X;
            Y = (int)MouseManager.Y;
            Title = nameof(ContextMenu);
            Visible = true;
            Closable = false;
            HasTitlebar = false;

            Contents.Clear(Color.LightGray);
            MouseMove();
        }

        private void MouseMove()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Contents.DrawString(0, i * 16, Items[i], Resources.Font_1x, Color.White);
            }

            RenderSystemStyleBorder();
        }

        public override void HandleRun()
        {
            base.HandleRun();
            if (MouseManager.LastMouseState == MouseState.Left && MouseManager.MouseState == MouseState.None &&
                !IsMouseOver)
            {
                Dispose();
            }
        }

        public override void HandleClick(MouseEventArgs e)
        {
            if (e.MouseState == MouseState.Left)
            {
                if (IsMouseOver)
                    Handle?.Invoke(Items[(MouseManager.Y - Y) / 16]);

                Dispose();
            }
        }
    }
}