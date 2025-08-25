using Gold.Graphics;

namespace GoOS.GUI.Apps;

public class Gimviewer : Window
{
    public static Canvas gim;
    public static string aeiou;
    public Gimviewer(byte[] image, int type) : base(0, 0, 800, 600, "Gimviewer")
    {
        SetDock(WindowDock.Auto);
        ProcessImg(image, type);
        Width = gim.Width;
        Height = gim.Height;
        Contents = gim;
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