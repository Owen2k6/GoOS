using System;
using Cosmos.System;
using GoOS.GUI.Models;
using Gold.Graphics;

namespace GoOS.GUI
{
    public class ContextMenu : Window
    {
        public string[] Items;
        public Action<string> Handle;

        // State: ignore the first Left->None transition (the opener)
        private bool suppressNextOutsideRelease = true;
        private MouseState lastState = MouseState.None;

        // Existing API: at mouse
        public static ContextMenu Show(string[] items, ushort width, Action<string> handle)
        {
            var cm = new ContextMenu(items, width) { Handle = handle };
            WindowManager.RemoveWindowByTitle(nameof(ContextMenu));
            WindowManager.AddWindow(cm);
            return cm;
        }

        // NEW API: at explicit screen coords (for dropdowns under buttons)
        public static ContextMenu ShowAt(int x, int y, string[] items, ushort width, Action<string> handle)
        {
            var cm = new ContextMenu(items, width, x, y) { Handle = handle };
            WindowManager.RemoveWindowByTitle(nameof(ContextMenu));
            WindowManager.AddWindow(cm);
            return cm;
        }

        // Existing ctor (spawn at mouse)
        public ContextMenu(string[] items, ushort width)
        {
            Items = items;
            WindowManager.MouseMove = MouseMove;

            Contents = new Canvas(width, Convert.ToUInt16(items.Length * 16 + 2));
            X = (int)MouseManager.X;
            Y = (int)MouseManager.Y;
            Title = nameof(ContextMenu);
            Visible = true;
            Closable = false;
            HasTitlebar = false;

            Contents.Clear(Color.LightGray);
            MouseMove();

            lastState = MouseManager.MouseState; // initialise tracker
            suppressNextOutsideRelease = true;   // ignore opener release
        }

        // NEW ctor (spawn at explicit x,y)
        public ContextMenu(string[] items, ushort width, int x, int y)
        {
            Items = items;
            WindowManager.MouseMove = MouseMove;

            Contents = new Canvas(width, Convert.ToUInt16(items.Length * 16 + 2));
            X = x;
            Y = y;
            Title = nameof(ContextMenu);
            Visible = true;
            Closable = false;
            HasTitlebar = false;

            Contents.Clear(Color.LightGray);
            MouseMove();

            lastState = MouseManager.MouseState; // initialise tracker
            suppressNextOutsideRelease = true;   // ignore opener release
        }

        private void MouseMove()
        {
            for (int i = 0; i < Items.Length; i++)
                Contents.DrawString(0, i * 16, Items[i], Resources.Font_1x, Color.White);

            RenderSystemStyleBorder();
        }

        public override void HandleRun()
        {
            base.HandleRun();

            // Detect Left -> None transitions (mouse release)
            var cur = MouseManager.MouseState;
            if (lastState == MouseState.Left && cur == MouseState.None)
            {
                if (suppressNextOutsideRelease)
                {
                    // Ignore the first release after opening
                    suppressNextOutsideRelease = false;
                }
                else
                {
                    // Now apply normal outside-click dismissal
                    if (!IsMouseOver)
                    {
                        Dispose();
                        return;
                    }
                }
            }
            lastState = cur;
        }

        public override void HandleClick(MouseEventArgs e)
        {
            if (e.MouseState == MouseState.Left)
            {
                if (IsMouseOver)
                {
                    int index = (int)((MouseManager.Y - Y) / 16); // cast long->int
                    if (index >= 0 && index < Items.Length)
                        Handle?.Invoke(Items[index]);
                }
                Dispose();
            }
        }
    }
}
