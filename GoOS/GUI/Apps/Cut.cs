using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using System.IO;
using System.Threading;

namespace GoOS.GUI.Apps;

public class Cut : Window
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.CUT.bmp")]
    private static byte[] cutIconRaw;

    private static Canvas cutIcon = Image.FromBitmap(cutIconRaw, false);

    public Cut()
    {
        Contents = new Canvas(148, 150);
        Title = "Cut";
        Visible = true;
        Closable = false;
        Unkillable = true;
        SetDock(WindowDock.Auto);

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();
        
        Contents.DrawImage(0, 0, cutIcon, true);
    }
}