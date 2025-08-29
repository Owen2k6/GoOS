using Gold.Graphics;
using IL2CPU.API.Attribs;

namespace GoOS.GUI.Apps.GoWeb;

internal static class GoWebResources
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.ToolbarBackground.bmp")]
    private static byte[] toolbarBackgroundRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.Go.bmp")]
    private static byte[] goRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.Home.bmp")]
    private static byte[] homeRaw;

    internal static Canvas toolbarBackground = Image.FromBitmap(toolbarBackgroundRaw);
    internal static Canvas go = Image.FromBitmap(goRaw);
    internal static Canvas home = Image.FromBitmap(homeRaw);
}