﻿using IL2CPU.API.Attribs;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;

namespace GoOS
{
    public enum ResourceType
    {
        All,
        Normal,
        Priority,
        Fonts,
        OOBE
    }

    public class Resources
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.oobe.bmp")]
        static byte[] OOBEmainraw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.oobebg.bmp")]
        static byte[] OOBEblankraw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.Credits05.bmp")]
        static byte[] easterEggRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_1x.btf")]
        static byte[] font_1x_raw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_2x.btf")]
        static byte[] font_2x_raw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoIDE.run.bmp")]
        static byte[] runRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.gterm.bmp")]
        static byte[] gtermIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.clock.bmp")]
        static byte[] clockIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskManager.bmp")]
        static byte[] taskmanIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.ide.bmp")]
        static byte[] ideIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.idesmall.bmp")]
        static byte[] ideIconSmallRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.CUT.bmp")]
        static byte[] cutIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.folder.bmp")]
        static byte[] folderIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.file.bmp")]
        static byte[] fileIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.desktopwallpaper.bmp")]
        static byte[] backgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.boot.bmp")]
        static byte[] bootbackgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.bootlogo.bmp")]
        static byte[] bootlogoRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.NEW.bmp")]
        static byte[] NewIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.BIN.bmp")]
        static byte[] BinIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.CHILD.bmp")]
        static byte[] ChildIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.PARENT.bmp")]
        static byte[] ParentIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.MOVE.bmp")]
        static byte[] MoveIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.COPY.bmp")]
        static byte[] copyIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.PASTE.bmp")]
        static byte[] pasteIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.REFRESH.bmp")]
        static byte[] refIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.LOADINNOTEPAD.bmp")]
        static byte[] linIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.aboutGoOS.bmp")]
        static byte[] aboutbgRAW;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.SAVE.bmp")]
        static byte[] saveIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.question.bmp")]
        static byte[] questionRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.colors.bmp")]
        static byte[] colorTableRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.brush.bmp")]
        static byte[] brushRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.bucket.bmp")]
        static byte[] bucketRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.text.bmp")]
        static byte[] textRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse_text.bmp")]
        static byte[] mouse_textRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Paint.rubber.bmp")]
        static byte[] rubberRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.user.bmp")]
        static byte[] userImageRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.shutdown.bmp")]
        static byte[] shutdownIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.Welcome.bmp")]
        static byte[] welcomeImageRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.info.bmp")]
        static byte[] infoIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.drum.bmp")]
        static byte[] drumIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.warning.bmp")]
        static byte[] warningIconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton.bmp")]
        static byte[] closeButtonRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton_hover.bmp")]
        static byte[] closeButtonHoverRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton_pressed.bmp")]
        static byte[] closeButtonPressedRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Maximize.bmp")]
        static byte[] maximiseRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Hovered.bmp")]
        static byte[] maximizeHoverRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Pressed.bmp")]
        static byte[] maximizePressedRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize.bmp")]
        static byte[] minimiseRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Hovered.bmp")]
        static byte[] minimiseHoverRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Pressed.bmp")]
        static byte[] minimisePressedRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStore.bmp")]
        static byte[] GoStoreRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreSoon.bmp")]
        static byte[] GoStoreSoonRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreicon.bmp")]
        static byte[] GoStoreiconRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.StoreButton.bmp")]
        static byte[] StoreButtonRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreinfoboard.bmp")]
        static byte[] GoStoreinfoboardRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonBlue.bmp")]
        static byte[] GoStoreButtonBlueRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonGreen.bmp")]
        static byte[] GoStoreButtonGreenRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreButtonRed.bmp")]
        static byte[] GoStoreButtonRedRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoStoreDescFrame.bmp")]
        static byte[] GoStoreDescFrameRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.appbackground.bmp")]
        static byte[] appbackgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowleft.bmp")]
        static byte[] arrowleftRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowright.bmp")]
        static byte[] arrowrightRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.arrowup.bmp")]
        static byte[] arrowupRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.drive.bmp")]
        static byte[] driveRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.drive_locked.bmp")]
        static byte[] drive_lockedRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.header.bmp")]
        static byte[] headerRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.FileManager.sidebar.bmp")]
        static byte[] sidebarRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskStart.startmenuBG.bmp")]
        static byte[] startMenuBackgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskStart.start.bmp")]
        static byte[] startBackgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskStart.taskbar.bmp")]
        static byte[] taskbarBackgroundRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoWeb.bmp")]
        static byte[] goWebIconRaw;

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
        public static Canvas closeButtonHover;
        public static Canvas closeButtonPressed;
        public static Canvas maximize;
        public static Canvas maximizeHover;
        public static Canvas maximizePressed;
        public static Canvas minimise;
        public static Canvas minimiseHover;
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
        public static Font Font_1x = Font.Fallback;
        public static Font Font_2x = Font.Fallback;

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
                    easterEgg = Image.FromBitmap(easterEggRaw, false);
                    RunImage = Image.FromBitmap(runRaw, false);
                    gtermIcon = Image.FromBitmap(gtermIconRaw, false);
                    clockIcon = Image.FromBitmap(clockIconRaw, false);
                    taskmanIcon = Image.FromBitmap(taskmanIconRaw, false);
                    ideIcon = Image.FromBitmap(ideIconRaw, false);
                    ideIconSmall = Image.FromBitmap(ideIconSmallRaw, false);
                    cutIcon = Image.FromBitmap(cutIconRaw, false);
                    folderIcon = Image.FromBitmap(folderIconRaw, false);
                    fileIcon = Image.FromBitmap(fileIconRaw, false);
                    newIcon = Image.FromBitmap(NewIconRaw, false);
                    binIcon = Image.FromBitmap(BinIconRaw, false);
                    childIcon = Image.FromBitmap(ChildIconRaw, false);
                    parentIcon = Image.FromBitmap(ParentIconRaw, false);
                    moveIcon = Image.FromBitmap(MoveIconRaw, false);
                    copyIcon = Image.FromBitmap(copyIconRaw, false);
                    pasteIcon = Image.FromBitmap(pasteIconRaw, false);
                    refIcon = Image.FromBitmap(refIconRaw, false);
                    linIcon = Image.FromBitmap(linIconRaw, false);
                    abtbg = Image.FromBitmap(aboutbgRAW, false);
                    saveIcon = Image.FromBitmap(saveIconRaw, false);
                    question = Image.FromBitmap(questionRaw, false);
                    colorTable = Image.FromBitmap(colorTableRaw, false);
                    brush = Image.FromBitmap(brushRaw, false);
                    bucket = Image.FromBitmap(bucketRaw, false);
                    text = Image.FromBitmap(textRaw, false);
                    mouse_text = Image.FromBitmap(mouse_textRaw, false);
                    rubber = Image.FromBitmap(rubberRaw, false);
                    userImage = Image.FromBitmap(userImageRaw, false);
                    shutdownIcon = Image.FromBitmap(shutdownIconRaw, false);
                    welcomeImage = Image.FromBitmap(welcomeImageRaw, false);
                    infoIcon = Image.FromBitmap(infoIconRaw, false);
                    warningIcon = Image.FromBitmap(warningIconRaw, false);
                    GoStore = Image.FromBitmap(GoStoreRaw, false);
                    GoStoreSoon = Image.FromBitmap(GoStoreSoonRaw, false);
                    GoStoreicon = Image.FromBitmap(GoStoreiconRaw, false);
                    StoreButton = Image.FromBitmap(StoreButtonRaw, false);
                    GoStoreinfoboard = Image.FromBitmap(GoStoreinfoboardRaw, false);
                    GoStoreButtonBlue = Image.FromBitmap(GoStoreButtonBlueRaw, false);
                    GoStoreButtonGreen = Image.FromBitmap(GoStoreButtonGreenRaw, false);
                    GoStoreButtonRed = Image.FromBitmap(GoStoreButtonRedRaw, false);
                    GoStoreDescFrame = Image.FromBitmap(GoStoreDescFrameRaw, false);
                    appbackground = Image.FromBitmap(appbackgroundRaw, false);
                    arrowleft = Image.FromBitmap(arrowleftRaw, false);
                    arrowright = Image.FromBitmap(arrowrightRaw, false);
                    arrowup = Image.FromBitmap(arrowupRaw, false);
                    drive = Image.FromBitmap(driveRaw, false);
                    drive_locked = Image.FromBitmap(drive_lockedRaw, false);
                    header = Image.FromBitmap(headerRaw, false);
                    sidebar = Image.FromBitmap(sidebarRaw, false);
                    startMenuBackground = Image.FromBitmap(startMenuBackgroundRaw, false);
                    startBackground = Image.FromBitmap(startBackgroundRaw, false);
                    taskbarBackground = Image.FromBitmap(taskbarBackgroundRaw, false);
                    goWebIcon = Image.FromBitmap(goWebIconRaw, false);
                    break;

                case ResourceType.Priority:
                    closeButton = Image.FromBitmap(closeButtonRaw, false);
                    closeButtonHover = Image.FromBitmap(closeButtonHoverRaw, false);
                    closeButtonPressed = Image.FromBitmap(closeButtonPressedRaw, false);
                    maximize = Image.FromBitmap(maximiseRaw, false);
                    maximizeHover = Image.FromBitmap(maximizeHoverRaw, false);
                    maximizePressed = Image.FromBitmap(maximizePressedRaw, false);
                    minimise = Image.FromBitmap(minimiseRaw, false);
                    minimiseHover = Image.FromBitmap(minimiseHoverRaw, false);
                    minimisePressed = Image.FromBitmap(minimisePressedRaw, false);
                    drumIcon = Image.FromBitmap(drumIconRaw, false);
                    background = Image.FromBitmap(backgroundRaw, false);
                    bootbackground = Image.FromBitmap(bootbackgroundRaw, false);
                    bootlogo = Image.FromBitmap(bootlogoRaw, false);
                    break;

                case ResourceType.Fonts:
                    Font_1x = new Font(font_1x_raw, 16);
                    Font_2x = new Font(font_2x_raw, 32);
                    break;

                case ResourceType.OOBE:
                    OOBEmain = Image.FromBitmap(OOBEmainraw, false);
                    OOBEblank = Image.FromBitmap(OOBEblankraw, false);
                    break;
            }
        }
    }
}