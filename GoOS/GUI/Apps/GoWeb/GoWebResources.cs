using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoWeb
{
    internal static class GoWebResources
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.ToolbarBackground.bmp")] static byte[] toolbarBackgroundRaw;
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.Go.bmp")] static byte[] goRaw;
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.Home.bmp")] static byte[] homeRaw;
        internal static Canvas toolbarBackground = Image.FromBitmap(toolbarBackgroundRaw, false);
        internal static Canvas go = Image.FromBitmap(goRaw, false);
        internal static Canvas home = Image.FromBitmap(homeRaw, false);
    }
}
