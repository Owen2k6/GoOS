using IL2CPU.API.Attribs;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI
{
    public static class Fonts
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_1x.btf")] private static byte[] font_1x_raw;
        public static Font Font_1x = new Font(font_1x_raw, 16);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_2x.btf")] private static byte[] font_2x_raw;
        public static Font Font_2x = new Font(font_2x_raw, 32);

        public static void Generate()
        {
            Font_1x = new Font(font_1x_raw, 16);
            Font_2x = new Font(font_2x_raw, 32);
        }
    }
}
