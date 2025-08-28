using GoGL.Graphics;
using GoGL.Graphics.Fonts;
using IL2CPU.API.Attribs;

namespace GoOS;

public enum ResourceType
{
    All,
    Boot,
    Normal,
    Priority,
    Fonts,
    OOBE
}

public class Resources
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Menubar.background.bmp")]
    private static byte[] menubarBackgroundRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.oobe.bmp")]
    private static byte[] OOBEmainraw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.oobebg.bmp")]
    private static byte[] OOBEblankraw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Credits05.bmp")]
    private static byte[] easterEggRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_1x.btf")]
    private static byte[] font_1x_raw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_2x.btf")]
    private static byte[] font_2x_raw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.MicrosoftSansSerif16.btf")]
    private static byte[] SansSerif16Raw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.MSUIGothic16.btf")]
    private static byte[] UIGothic16Raw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Geneva.btf")]
    private static byte[] genevaRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Charcoal.btf")]
    private static byte[] charcoalRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.Chicago.btf")]
    private static byte[] chicagoRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoIDE.run.bmp")]
    private static byte[] runRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.gterm.bmp")]
    private static byte[] gtermIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.clock.bmp")]
    private static byte[] clockIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskManager.bmp")]
    private static byte[] taskmanIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.ide.bmp")]
    private static byte[] ideIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.idesmall.bmp")]
    private static byte[] ideIconSmallRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.CUT.bmp")]
    private static byte[] cutIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.folder.bmp")]
    private static byte[] folderIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.file.bmp")]
    private static byte[] fileIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.desktopwallpaper.bmp")]
    private static byte[] backgroundRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.bootlogo.bmp")]
    private static byte[] bootlogoRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.NEW.bmp")]
    private static byte[] NewIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.BIN.bmp")]
    private static byte[] BinIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.CHILD.bmp")]
    private static byte[] ChildIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.PARENT.bmp")]
    private static byte[] ParentIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.MOVE.bmp")]
    private static byte[] MoveIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.COPY.bmp")]
    private static byte[] copyIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.PASTE.bmp")]
    private static byte[] pasteIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.REFRESH.bmp")]
    private static byte[] refIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.LOADINNOTEPAD.bmp")]
    private static byte[] linIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.aboutGoOS.bmp")]
    private static byte[] aboutbgRAW;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.SAVE.bmp")]
    private static byte[] saveIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.question.bmp")]
    private static byte[] questionRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.colors.bmp")]
    private static byte[] colorTableRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.brush.bmp")]
    private static byte[] brushRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.bucket.bmp")]
    private static byte[] bucketRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.text.bmp")]
    private static byte[] textRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse_text.bmp")]
    private static byte[] mouse_textRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.rubber.bmp")]
    private static byte[] rubberRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.shutdown.bmp")]
    private static byte[] shutdownIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.info.bmp")]
    private static byte[] infoIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.warning.bmp")]
    private static byte[] warningIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Close.bmp")]
    private static byte[] closeButtonRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.ClosePressed.bmp")]
    private static byte[] closeButtonPressedRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize.bmp")]
    private static byte[] minimiseRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.MinimizePressed.bmp")]
    private static byte[] minimisePressedRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Maximize.bmp")]
    private static byte[] maximiseRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.MaximizedPressed.bmp")]
    private static byte[] maximisePressedRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStore.bmp")]
    private static byte[] GoStoreRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreSoon.bmp")]
    private static byte[] GoStoreSoonRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreicon.bmp")]
    private static byte[] GoStoreiconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.StoreButton.bmp")]
    private static byte[] StoreButtonRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreinfoboard.bmp")]
    private static byte[] GoStoreinfoboardRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonBlue.bmp")]
    private static byte[] GoStoreButtonBlueRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonGreen.bmp")]
    private static byte[] GoStoreButtonGreenRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonRed.bmp")]
    private static byte[] GoStoreButtonRedRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreDescFrame.bmp")]
    private static byte[] GoStoreDescFrameRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.appbackground.bmp")]
    private static byte[] appbackgroundRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowleft.bmp")]
    private static byte[] arrowleftRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowright.bmp")]
    private static byte[] arrowrightRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowup.bmp")]
    private static byte[] arrowupRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.drive.bmp")]
    private static byte[] driveRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.drive_locked.bmp")]
    private static byte[] drive_lockedRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.header.bmp")]
    private static byte[] headerRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.sidebar.bmp")]
    private static byte[] sidebarRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskStart.taskbar.bmp")]
    private static byte[] taskbarBackgroundRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.bmp")]
    private static byte[] goWebIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Settings.Background.bmp")]
    private static byte[] SBGRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Settings.Unknown.bmp")]
    private static byte[] UnknownRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Settings.BackgroundMenu.bmp")]
    private static byte[] SBGMRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Settings.ButtonSelection.bmp")]
    private static byte[] SBGBSRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Settings.Back.bmp")]
    private static byte[] SBBBRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")]
    private static byte[] mouseRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse_click.bmp")]
    private static byte[] mouseClickRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.error.bmp")]
    private static byte[] errorIconRaw;


    //[ManifestResourceStream(ResourceName = "GoOS.Resources.Fragment.acf")]
    //static byte[] FragmentRaw;

    public static Canvas easterEgg;
    public static Canvas RunImage;
    public static Canvas gtermIcon;
    public static Canvas clockIcon;
    public static Canvas taskmanIcon;
    public static Canvas ideIcon;
    public static Canvas ideIconSmall;
    public static Canvas cutIcon;
    public static Canvas folderIcon;
    public static Canvas fileIcon;
    public static Canvas background;
    public static Canvas bootbackground;
    public static Canvas bootlogo;
    public static Canvas newIcon;
    public static Canvas binIcon;
    public static Canvas childIcon;
    public static Canvas parentIcon;
    public static Canvas moveIcon;
    public static Canvas copyIcon;
    public static Canvas pasteIcon;
    public static Canvas refIcon;
    public static Canvas linIcon;
    public static Canvas abtbg;
    public static Canvas saveIcon;
    public static Canvas question;
    public static Canvas colorTable;
    public static Canvas brush;
    public static Canvas bucket;
    public static Canvas text;
    public static Canvas mouse_text;
    public static Canvas rubber;
    public static Canvas userImage;
    public static Canvas shutdownIcon;
    public static Canvas welcomeImage;
    public static Canvas infoIcon;
    public static Canvas drumIcon;
    public static Canvas warningIcon;
    public static Canvas closeButton;
    public static Canvas closeButtonPressed;
    public static Canvas maximise;
    public static Canvas maximisePressed;
    public static Canvas minimise;
    public static Canvas minimisePressed;
    public static Canvas GoStore;
    public static Canvas GoStoreSoon;
    public static Canvas GoStoreicon;
    public static Canvas StoreButton;
    public static Canvas OOBEmain;
    public static Canvas OOBEblank;
    public static Canvas GoStoreinfoboard;
    public static Canvas GoStoreButtonBlue;
    public static Canvas GoStoreButtonGreen;
    public static Canvas GoStoreButtonRed;
    public static Canvas GoStoreDescFrame;
    public static Canvas appbackground;
    public static Canvas arrowleft;
    public static Canvas arrowright;
    public static Canvas arrowup;
    public static Canvas drive;
    public static Canvas drive_locked;
    public static Canvas header;
    public static Canvas sidebar;
    public static Canvas startMenuBackground;
    public static Canvas startBackground;
    public static Canvas taskbarBackground;
    public static Canvas goWebIcon;
    public static Canvas SBG;
    public static Canvas Unknown;
    public static Canvas SBGM;
    public static Canvas SBGBS;
    public static Canvas SBBB;
    public static Canvas menubarBackground;
    public static Font Font_1x;
    public static Font Font_2x;
    public static Font SansSerif16;
    public static Font UIGothic16;
    public static Font Geneva;
    public static Font Charcoal;
    public static Font Chicago;
    public static Canvas Mouse;
    private static Canvas MouseClick;
    public static Canvas errorIcon;

    public static void Generate(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.All:
                Generate(ResourceType.Normal);
                Generate(ResourceType.Fonts);
                Generate(ResourceType.OOBE);
                break;

            case ResourceType.Normal:
                easterEgg = Image.FromBitmap(easterEggRaw);
                RunImage = Image.FromBitmap(runRaw);
                gtermIcon = Image.FromBitmap(gtermIconRaw);
                clockIcon = Image.FromBitmap(clockIconRaw);
                taskmanIcon = Image.FromBitmap(taskmanIconRaw);
                ideIcon = Image.FromBitmap(ideIconRaw);
                ideIconSmall = Image.FromBitmap(ideIconSmallRaw);
                cutIcon = Image.FromBitmap(cutIconRaw);
                folderIcon = Image.FromBitmap(folderIconRaw);
                fileIcon = Image.FromBitmap(fileIconRaw);
                newIcon = Image.FromBitmap(NewIconRaw);
                binIcon = Image.FromBitmap(BinIconRaw);
                childIcon = Image.FromBitmap(ChildIconRaw);
                parentIcon = Image.FromBitmap(ParentIconRaw);
                moveIcon = Image.FromBitmap(MoveIconRaw);
                copyIcon = Image.FromBitmap(copyIconRaw);
                pasteIcon = Image.FromBitmap(pasteIconRaw);
                refIcon = Image.FromBitmap(refIconRaw);
                linIcon = Image.FromBitmap(linIconRaw);
                abtbg = Image.FromBitmap(aboutbgRAW);
                saveIcon = Image.FromBitmap(saveIconRaw);
                question = Image.FromBitmap(questionRaw);
                colorTable = Image.FromBitmap(colorTableRaw);
                brush = Image.FromBitmap(brushRaw);
                bucket = Image.FromBitmap(bucketRaw);
                text = Image.FromBitmap(textRaw);
                mouse_text = Image.FromBitmap(mouse_textRaw);
                rubber = Image.FromBitmap(rubberRaw);
                infoIcon = Image.FromBitmap(infoIconRaw);
                warningIcon = Image.FromBitmap(warningIconRaw);
                GoStore = Image.FromBitmap(GoStoreRaw);
                GoStoreSoon = Image.FromBitmap(GoStoreSoonRaw);
                GoStoreicon = Image.FromBitmap(GoStoreiconRaw);
                StoreButton = Image.FromBitmap(StoreButtonRaw);
                GoStoreinfoboard = Image.FromBitmap(GoStoreinfoboardRaw);
                GoStoreButtonBlue = Image.FromBitmap(GoStoreButtonBlueRaw);
                GoStoreButtonGreen = Image.FromBitmap(GoStoreButtonGreenRaw);
                GoStoreButtonRed = Image.FromBitmap(GoStoreButtonRedRaw);
                GoStoreDescFrame = Image.FromBitmap(GoStoreDescFrameRaw);
                appbackground = Image.FromBitmap(appbackgroundRaw);
                arrowleft = Image.FromBitmap(arrowleftRaw);
                arrowright = Image.FromBitmap(arrowrightRaw);
                arrowup = Image.FromBitmap(arrowupRaw);
                drive = Image.FromBitmap(driveRaw);
                drive_locked = Image.FromBitmap(drive_lockedRaw);
                header = Image.FromBitmap(headerRaw);
                sidebar = Image.FromBitmap(sidebarRaw);
                taskbarBackground = Image.FromBitmap(taskbarBackgroundRaw);
                goWebIcon = Image.FromBitmap(goWebIconRaw);
                SBG = Image.FromBitmap(SBGRaw);
                Unknown = Image.FromBitmap(UnknownRaw);
                SBGM = Image.FromBitmap(SBGMRaw);
                SBGBS = Image.FromBitmap(SBGBSRaw);
                SBBB = Image.FromBitmap(SBBBRaw);
                errorIcon = Image.FromBitmap(errorIconRaw);
                break;

            case ResourceType.Priority:
                closeButton = Image.FromBitmap(closeButtonRaw);
                closeButtonPressed = Image.FromBitmap(closeButtonPressedRaw);
                maximise = Image.FromBitmap(maximiseRaw);
                maximisePressed = Image.FromBitmap(maximiseRaw);
                minimise = Image.FromBitmap(minimiseRaw);
                minimisePressed = Image.FromBitmap(minimisePressedRaw);
                background = Image.FromBitmap(backgroundRaw);
                menubarBackground = Image.FromBitmap(menubarBackgroundRaw);
                Mouse = Image.FromBitmap(mouseRaw);
                MouseClick = Image.FromBitmap(mouseClickRaw);
                break;

            case ResourceType.Boot:
                bootlogo = Image.FromBitmap(bootlogoRaw);
                break;

            case ResourceType.Fonts:
                Font_1x = new Font(font_1x_raw, 16);
                Font_2x = new Font(font_2x_raw, 32);
                SansSerif16 = new Font(SansSerif16Raw, 32);
                UIGothic16 = new Font(UIGothic16Raw, 32);
                Geneva = new Font(genevaRaw, 16);
                Charcoal = new Font(charcoalRaw, 16);
                Chicago = new Font(chicagoRaw, 16);
                break;

            case ResourceType.OOBE:
                OOBEmain = Image.FromBitmap(OOBEmainraw);
                OOBEblank = Image.FromBitmap(OOBEblankraw);
                break;
        }
    }
}