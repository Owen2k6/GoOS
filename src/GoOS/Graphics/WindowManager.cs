using System;
using Gold.Hardware.GPU;

namespace GoOS.Graphics
{
    public static class WindowManager
    {
        public static Display Screen = Display.GetDisplay(1024, 768);

        public static void RenderBootscreen()
        {
            // TODO: finish this, sync Gold to use latest GrapeGL, update SVGAIITerminal
            Screen.DrawImage(512 - (Resources.Logo.Width / 2), 384 - (Resources.Logo.Height), Resources.Logo, true);
        }
    }
}