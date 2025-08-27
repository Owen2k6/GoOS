using System;
using Cosmos.System;
using Gold.Graphics;

namespace GoOS.GUI
{
    internal class ContextMenu : Window
    {
        public string[] Items;
        public Action<string> Handle;

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
                Contents.DrawString(0, i * 16, Items[i], Resources.Font_1x, Color.White);
            
            base.Render();
        }

        internal override void HandleRun()
        { //Kernel.ProcessScheduler.Iterations >= Kernel.ProcessScheduler.IPS / 2
            if (MouseManager.MouseState == MouseState.Left)
            {
                if (IsMouseOver())
                {
                    int index = (int)((MouseManager.Y - Y) / 16); // cast long->int
                    if (index >= 0 && index < Items.Length)
                        Handle?.Invoke(Items[index]);
                }
                else Dispose();
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