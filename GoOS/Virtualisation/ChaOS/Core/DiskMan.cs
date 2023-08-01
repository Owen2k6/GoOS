using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using static ChaOS.Core;
using Console = BetterConsole;
using static ConsoleColorEx;

namespace ChaOS
{
    public class DiskManager
    {
        public const string systempath = @"0:\SYSTEM";
        public const string rootdir = @"0:\";
        public static bool disk = true;

        public class Files
        {
            public const string userfile = @"0:\SYSTEM\USERFILE.SYS";
            public const string colorfile = @"0:\SYSTEM\COLOR.SYS";
        };

        public static void InitFS(CosmosVFS fs)
        {
            VFSManager.RegisterVFS(fs);
            try { Directory.GetFiles(rootdir); }
            catch { disk = false; }
        }

        public static void LoadSettings()
        {
            if (disk)
            {
                if (Directory.Exists(systempath))
                {
                    if (File.Exists(Files.userfile)) GoOS.Kernel.username = File.ReadAllText(Files.userfile);
                    //if (File.Exists(Files.colorfile)) SetScreenColor((ConsoleColor)File.ReadAllBytes(Files.colorfile)[0], (ConsoleColor)File.ReadAllBytes(Files.colorfile)[1], false);
                }
            }
        }

        public static void SaveChangesToDisk()
        {
            clog("Writing changes to disk...", Gray);
            Directory.CreateDirectory(systempath);
            File.WriteAllText(Files.userfile, GoOS.Kernel.username);
            //File.WriteAllBytes(Files.colorfile, new byte[] { (byte)GoOS.GUI.Apps.ChaOS_VM.VMTERM.BackgroundColor, (byte)GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor });
        }
    }
}
