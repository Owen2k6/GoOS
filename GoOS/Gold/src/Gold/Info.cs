using Gold.Graphics;
using IL2CPU.API.Attribs;

namespace Gold;

public static class Info
{
    #region Fields

    #pragma warning disable CS8604

    [ManifestResourceStream(ResourceName = "Gold.Resources.Gold.bmp")] static byte[] _rawLogo;
    public static Canvas Logo = Image.FromBitmap(_rawLogo, false);

    #pragma warning restore CS8604

    public const string Version = "0.0.1"; //The version of GOLD.
    public const string ApiVersion = "1.3"; //The PrismAPI version which GOLD is based off.

    #endregion
}