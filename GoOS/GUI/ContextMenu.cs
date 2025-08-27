using System;
using System.ComponentModel;
using Cosmos.System;
using Gold.Graphics;

namespace GoOS.GUI
{
    internal class ContextMenu : Window
    {
        public string[] Items;
        public Action<string> Handle;
        private bool _pressed = false;
        private int _pressedIndex = -1;

        // State: ignore the first Left->None transition (the opener)
        //private bool suppressNextOutsideRelease = true;
        //private MouseState lastState = MouseState.None;

        // Existing API: at mouse
        public static ContextMenu Show(string[] items, ushort width, Action<string> handle)
        {
            var cm = new ContextMenu(items, width) { Handle = handle };
            Kernel.ProcessScheduler.AddProcess(cm);
            return cm;
        }

        // NEW API: at explicit screen coords (for dropdowns under buttons)
        public static ContextMenu ShowAt(int x, int y, string[] items, ushort width, Action<string> handle)
        {
            var cm = new ContextMenu(items, width, x, y) { Handle = handle };
            Kernel.ProcessScheduler.AddProcess(cm);
            return cm;
        }

        // Existing ctor (spawn at mouse)
        public ContextMenu(string[] items, ushort width) : base((int)MouseManager.X, (int)MouseManager.Y, width,
            Convert.ToUInt16(items.Length * 16 + 2), "contextMenu", true)
        {
            Items = items;
            //WindowManager.MouseMove = MouseMove;

            //Visible = true;
            //Closable = false;
            //HasTitlebar = false;

            Render();

            //lastState = MouseManager.MouseState; // initialise tracker
            //suppressNextOutsideRelease = true; // ignore opener release
        }

        // NEW ctor (spawn at explicit x,y)
        public ContextMenu(string[] items, ushort width, int x, int y) : base(x, y,
            width, Convert.ToUInt16(items.Length * 16 + 2), "contextMenu", true)
        {
            Items = items;
            //WindowManager.MouseMove = MouseMove;

            //Visible = true;
            //Closable = false;
            //HasTitlebar = false;

            Render();

            //lastState = MouseManager.MouseState; // initialise tracker
            //suppressNextOutsideRelease = true; // ignore opener release
        }

        internal override void Render()
        {
            Contents.Clear(Color.LightGray);
            for (int i = 0; i < Items.Length; i++)
            {
                if (i == _pressedIndex)
                    Contents.DrawString(1, i * 16 + 1, Items[i], Resources.Font_1x, Color.White);
                else
                    Contents.DrawString(0, i * 16, Items[i], Resources.Font_1x, Color.White, false, true);
            }

            base.Render();
        }

        internal override void HandleRun()
        {
            if (IsMouseOver())
            {
                if (_pressed) {
                    _pressedIndex = (int)((MouseManager.Y - Y) / 16);
                }
                
                if (MouseManager.MouseState == MouseState.Left && !_pressed)
                {
                    _pressed = true;
                }

                if (MouseManager.MouseState == MouseState.None && _pressed)
                {
                    if (_pressedIndex >= 0 && _pressedIndex < Items.Length)
                    {
                        Handle?.Invoke(Items[_pressedIndex]);
                        Dispose();
                    }

                    _pressed = false;
                    _pressedIndex = -1;
                }
            }
            else 
            {
                if (MouseManager.MouseState == MouseState.None && _pressed)
                {
                    _pressed = false;
                    _pressedIndex = -1;
                }
                
                if (MouseManager.MouseState == MouseState.Left)
                    Dispose();
                
            }
        }

        /*public override void HandleClick(MouseEventArgs e)
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
        }*/
    }
}