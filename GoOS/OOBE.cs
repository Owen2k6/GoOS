using System;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;
using static GoOS.Themes.ThemeManager;
using Console = BetterConsole;
using GoOS.Themes;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using System.Collections.Generic;
using static GoOS.Resources;

namespace GoOS
{
    public static class OOBE
    {
        private static string username, computerName, theme;
        private static byte videoMode;

        public static void Launch()
        {
            Resources.Generate(ResourceType.OOBE);
            
            for (int i = 0; i < 9; i++)
                ShowPage(i + 1);

            //Sys.Power.Reboot();
        }

        public static void ShowPage(int page)
        {   int tries = 0;
            
            switch (page)
            {
                case 1:
                    Console.Canvas.DrawImage(0, 0, setupWelcome, false);
                    Console.Render();

                    ReadAgain1:
                    var key1 = Console.ReadKey(true);
                    if (key1.Key != ConsoleKey.Enter) goto ReadAgain1;

                    break;

                case 2:
                    Console.Canvas.DrawImage(0, 0, setupTOU, false);
                    Console.Render();

                    ReadAgain2:
                    var key2 = Console.ReadKey(true);
                    if (key2.Key != ConsoleKey.F8) goto ReadAgain2;
                    break;

                case 3:
                    Console.Canvas.DrawImage(0, 0, setupUA, false);
                    Console.Render();

                    ReadAgain3:
                    var key3 = Console.ReadKey(true);
                    if (key3.Key != ConsoleKey.F9) goto ReadAgain3;
                    break;

                case 4:
                    Console.Canvas.DrawImage(0, 0, setupSupport, false);
                    Console.Render();

                    ReadAgain4:
                    var key4 = Console.ReadKey(true);
                    if (key4.Key != ConsoleKey.Enter) goto ReadAgain4;
                    break;

                case 5:
                    Console.Canvas.DrawImage(0, 0, setupUser, false);
                    Console.Render();

                    Console.SetCursorPosition(14, 8);
                    Console.Write("Username: ");
                    username = Console.ReadLine();
                    break;

                case 6:
                    Console.Canvas.DrawImage(0, 0, setupComputer, false);
                    Console.Render();

                    Console.SetCursorPosition(14, 7);
                    Console.Write("Computer Name: ");
                    computerName = Console.ReadLine();
                    break;

                case 7:
                    Console.Canvas.DrawImage(0, 0, setupTheme, false);
                    Console.Render();

                    int themeMenuSelectedButton = 0;

                    Refresh7:
                    string[] themes =
                    {
                        "default",
                        "mono",
                        "dark",
                        "light"
                    };

                    if (themeMenuSelectedButton > (themes.Length - 1))
                    {
                        themeMenuSelectedButton = 0;
                    }
                    else if (themeMenuSelectedButton < 0)
                    {
                        themeMenuSelectedButton = themes.Length - 1;
                    }

                    for (int i = 0; i < themes.Length; i++)
                    {
                        ControlPanel.DrawButton(themes[i], 14, 6 + i,
                            i == themeMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key7 = Console.ReadKey(true).Key;
                    switch (key7)
                    {
                        case ConsoleKey.Enter:
                            theme = "ThemeFile = " + @"0:\content\themes\" + themes[themeMenuSelectedButton] +
                                    ".gtheme";
                            break;

                        case ConsoleKey.UpArrow:
                            themeMenuSelectedButton--;
                            goto Refresh7;

                        case ConsoleKey.DownArrow:
                            themeMenuSelectedButton++;
                            goto Refresh7;

                        default:
                            goto Refresh7;
                    }

                    break;

                case 8:
                    Console.Canvas.DrawImage(0, 0, setupRes, false);
                    Console.Render();

                    List<(string, (ushort Width, ushort Height))> videoModes = new()
                    {
                        ("800 x 600", (800, 600)),
                        ("1280 x 720", (1280, 720)),
                        ("1920 x 1080", (1920, 1080))
                    };

                    int displayMenuSelectedButton = 0;

                    Refresh8:
                    if (displayMenuSelectedButton > videoModes.Count - 1)
                    {
                        displayMenuSelectedButton = 0;
                    }
                    else if (displayMenuSelectedButton < 0)
                    {
                        displayMenuSelectedButton = videoModes.Count - 1;
                    }

                    for (int i = 0; i < videoModes.Count; i++)
                    {
                        ControlPanel.DrawButton(videoModes[i].Item1, 14, 5 + i,
                            i == displayMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Enter:
                            if (videoModes[displayMenuSelectedButton].Item1 == "800 x 600") videoMode = 2;
                            if (videoModes[displayMenuSelectedButton].Item1 == "1280 x 720") videoMode = 4;
                            if (videoModes[displayMenuSelectedButton].Item1 == "1920 x 1080") videoMode = 6;
                            break;

                        case ConsoleKey.UpArrow:
                            displayMenuSelectedButton--;
                            goto Refresh8;

                        case ConsoleKey.DownArrow:
                            displayMenuSelectedButton++;
                            goto Refresh8;

                        default:
                            goto Refresh8;
                    }

                    break;

                case 9:
                    
                    
                    retry:
                    try
                    {
                        Directory.CreateDirectory(@"0:\content");
                        Directory.CreateDirectory(@"0:\content\sys");
                        Directory.CreateDirectory(@"0:\content\themes");
                        Directory.CreateDirectory(@"0:\content\prf");
                        Directory.CreateDirectory(@"0:\framework");
                        File.Create(@"0:\content\sys\option-showprotectedfiles.gms");
                        File.Create(@"0:\content\sys\option-editprotectedfiles.gms");
                        File.Create(@"0:\content\sys\option-deleteprotectedfiles.gms");
                        File.Create(@"0:\content\sys\setup.gms");
                        File.WriteAllText(@"0:\content\sys\version.gms", $"System.Version is set to {Kernel.version} \n Note to users reading this: DO NOT ALTER. IMPORTANT IF USER DATA NEEDS CONVERTING.");
                        File.WriteAllText(@"0:\content\sys\user.gms", $"username: {username}\ncomputername: {computerName}");
                        File.WriteAllBytes(@"0:\content\sys\resolution.gms", new byte[] { videoMode }); // Video mode 2: 1280x720
                        File.WriteAllText(@"0:\content\themes\default.gtheme", "Default = White\nBackground = Black\nStartup = DarkMagenta,Red,DarkRed\nWindowText = Cyan\nWindowBorder = Green\nErrorText = Red\nOther1 = Yellow");
                        File.WriteAllText(@"0:\content\themes\mono.gtheme", "Default = White\nBackground = Black\nStartup = White,White,White\nWindowText = White\nWindowBorder = White\nErrorText = White\nOther1 = White");
                        File.WriteAllText(@"0:\content\themes\dark.gtheme", "Default = Gray\nBackground = Black\nStartup = DarkGray,Gray,DarkGray\nWindowText = Gray\nWindowBorder = DarkGray\nErrorText = DarkGray\nOther1 = DarkGray");
                        File.WriteAllText(@"0:\content\themes\light.gtheme", "Default = Black\nBackground = White\nStartup = Black,Black,Black\nWindowText = Black\nWindowBorder = Black\nErrorText = Black\nOther1 = Black");
                        File.WriteAllText(@"0:\content\sys\theme.gms", "ThemeFile = " + theme);

                        tries++;
                    }
                    catch (Exception e)
                    {               
                        Console.SetCursorPosition(0,0);
                        
                        if (tries < 10)
                        {
                            goto retry;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(e);
                            Console.WriteLine("Press any key to shutdown.");
                            Console.ReadKey(true);
                            Sys.Power.Shutdown();
                        }
                    }

                    Console.Canvas.DrawImage(0, 0, setupFinal, false);
                    Console.Render();

                    ReadAgain9:
                    var key9 = Console.ReadKey(true);
                    if (key9.Key != ConsoleKey.Enter) goto ReadAgain9;
                    Sys.Power.Reboot();
                    break;
            }
        }
    }
}