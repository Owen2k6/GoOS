using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public abstract class Window
    {
        public Canvas Contents;
        public int X, Y; //This doesnt have to be a UInt16
        public string Title;
        public bool Visible;
        public bool Closeable;
        public bool Moving;

        public abstract void Update();
    }
}
