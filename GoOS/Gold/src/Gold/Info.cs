using Gold.Graphics;
using IL2CPU.API.Attribs;

namespace Gold;

public static class Info
{
    public const string Version = "0.0.2"; //The version of GOLD.
    public const string ApiVersion = "1.3"; //The PrismAPI version which GOLD is based off.

#pragma warning disable CS8604

    [ManifestResourceStream(ResourceName = "Gold.Resources.Gold.bmp")]
    private static byte[] _rawLogo;

    public static Canvas Logo = Image.FromBitmap(_rawLogo);

#pragma warning restore CS8604
}