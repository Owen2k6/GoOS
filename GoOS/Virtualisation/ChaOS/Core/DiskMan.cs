using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Gold.Graphics;

namespace GoOS.Virtualisation.ChaOS.Core
{
    public class DiskManager
    {
        public const string systempath = @"0:\SYSTEM";
        public const string rootdir = @"0:\";
        public bool disk = true;
        
        private Kernel parent;

        private SVGAIITerminal Console;

        public DiskManager(Kernel parent)
        {
            this.parent = parent;
            Console = parent.Console;
        }

        public class Files
        {
            public const string userfile = @"0:\SYSTEM\USERFILE.SYS";
            public const string colorfile = @"0:\SYSTEM\COLOR.SYS";
        };

        public void InitFS(CosmosVFS fs)
        {
            VFSManager.RegisterVFS(fs);
            try { Directory.GetFiles(rootdir); }
            catch { disk = false; }
        }

        public void LoadSettings()
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

        public void SaveChangesToDisk()
        {
            parent.core.clog("Writing changes to disk...", Color.LightGray);
            Directory.CreateDirectory(systempath);
            File.WriteAllText(Files.userfile, GoOS.Kernel.username);
            //File.WriteAllBytes(Files.colorfile, new byte[] { (byte)GoOS.GUI.Apps.ChaOS_VM.VMTERM.BackgroundColor, (byte)GoOS.GUI.Apps.ChaOS_VM.VMTERM.ForegroundColor });
        }
    }
}
