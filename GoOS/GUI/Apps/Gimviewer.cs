using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using System.IO;
using System.Threading;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Gimviewer : Window
{
    public static Canvas gim;
    public static string aeiou;
    public Gimviewer(byte[] image, int type)
    {
        
        Visible = true;
        Closable = true;
        Unkillable = false;
        SetDock(WindowDock.Auto);
        ProcessImg(image, type);
        Contents = gim;
        RenderSystemStyleBorder();
        Title = "Gimviewer! Currently viewing: "+aeiou;
    }

    public void ProcessImg(byte[] image, int type)
    {
        switch(type)
        {
            case 0:
                gim = Image.FromBitmap(image, false);
                aeiou = "Bitmap";
                break;
            case 1:
                gim = Image.FromPNG(image);
                aeiou = "PNG";
                break;
            case 2:
                gim = Image.FromPPM(image);
                aeiou = "PPM";
                break;
            case 3:
                gim = Image.FromTGA(image);
                aeiou = "TGA";
                break;
        }
    }
}