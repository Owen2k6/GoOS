using System.Collections.Generic;

namespace GoOS
{
    //DEPRECATED AS OF GOOS 1.5.3
    //THIS APP IS JUST A BACKEND NOW FOR THE MODERN SETTINGS APP
    public static class ControlPanel
    {
        private static bool isRunning = true;

        public static readonly List<(string, (ushort Width, ushort Height))> videoModes = new()
        {
            ("800  × 600 (4:3)", (800, 600)),
            ("1024 × 768 (4:3)", (1024, 768)),
            ("1280 × 960 (4:3)", (1280, 960)),
            ("1400 × 1050 (4:3)", (1400, 1050)),
            ("1600 × 1200 (4:3)", (1600, 1200)),
            ("1280 × 720 (16:9)", (1280, 720)),
            ("1280 × 800 (16:10)", (1280, 800)),
            ("1366 × 768 (16:9)", (1366, 768)),
            ("1440 × 900 (16:10)", (1440, 900)),
            ("1600 × 900 (16:9)", (1600, 900)),
            ("1680 × 1050 (16:10)", (1680, 1050)),
            ("1920 × 1080 (16:9)", (1920, 1080)),
            ("1920 × 1200 (16:10)", (1920, 1200)),
            ("2560 × 1440 (16:9)", (2560, 1440))
        };
    }
}