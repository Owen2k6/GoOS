using System.IO;
using Gold.Graphics;
using IL2CPU.API.Attribs;
using Gold.Graphics.Fonts;

namespace GoOS
{
    public static class Resources
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.Helvetica.acf")] static byte[] _rawHelvetica;
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GoOS.bmp")] static byte[] _rawLogo;

        public static AcfFontFace Plex = new AcfFontFace(new MemoryStream(_rawHelvetica));
        public static Canvas Logo = Image.FromBitmap(_rawLogo);
    }
}