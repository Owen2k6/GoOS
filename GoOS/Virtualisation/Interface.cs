using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.ConsoleColor;
using Sys = Cosmos.System;
using GoOS.Virtualisation;
using static ChaOS.Core;

namespace GoOS.Virtualisation
{
    //This is not to be used. mainly to create a basic image for developers to clone into their own .cs file and produce a "virtual" OS
    //You will have to modify a ton of shit from the OS in order to work with this interface.
    //A shutdown command is highly reccomended and should issue runmode = false;
    internal class Interface
    {
        public static bool runmode = false;

        public const string ver = "Release 1.2";
        public const string copyright = "Copyright (c) 2022 Goplex Studios";

        public static string username = "usr";

        public static string input;
        public static string inputBeforeLower;
        public static string inputCapitalized;

        public static void boot(string rootpath)
        {
            //BeforeRun bullshit goes here
            try
            {
                //log("Starting up ChaOS...");
                //InitFS(fs);
                //LoadSettings();

                Console.Clear();
                log("Welcome to...\n");
                clog("  ______   __                   ______    ______  \n /      \\ |  \\                 /      \\  /      \\ \n|  $$$$$$\\| $$____    ______  |  $$$$$$\\|  $$$$$$\\\n| $$   \\$$| $$    \\  |      \\ | $$  | $$| $$___\\$$\n| $$      | $$$$$$$\\  \\$$$$$$\\| $$  | $$ \\$$    \\ \n| $$   __ | $$  | $$ /      $$| $$  | $$ _\\$$$$$$\\\n| $$__/  \\| $$  | $$|  $$$$$$$| $$__/ $$|  \\__| $$\n \\$$    $$| $$  | $$ \\$$    $$ \\$$    $$ \\$$    $$\n  \\$$$$$$  \\$$   \\$$  \\$$$$$$$  \\$$$$$$   \\$$$$$$ ", DarkGreen);
                log("\n" + ver + "\n" + copyright + "\nType \"help\" to get started!");
                //if (!disk) log("No hard drive detected, ChaOS will continue without disk support.");
                log();
            }
            catch (Exception ex) { Crash(ex, true); }
            runmode = true;
            run(rootpath);
        }
        public static void run(string rtp) {
            
            while (runmode)
            {
                //run contents in here
            }
        
        }
    }
}
