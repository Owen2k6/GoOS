using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoOS.GUI;
using PrismAPI.Graphics;

namespace GoOS._9xCode
{
    public class FlexWindow : Window
    {
        public FlexWindow(string[] args)
        {
            Contents = new Canvas(Convert.ToUInt16(args[1]), Convert.ToUInt16(args[2]));
            Title = args[0];
            Visible = true;
            Closable = true;

            Contents.Clear(Color.White);
            RenderSystemStyleBorder();

            WindowManager.AddWindow(this);
        }
    }
}
