using System;
using Sys = Cosmos.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CosmosTTF;
using System.Threading.Tasks;
using System.IO;

namespace TechOS.System
{
    internal class ConfigFile
    {
        public static string wallpath;
        public static string osname;
        public static string osversion;
        public static string canvastype;
        public static void LoadSystemConfig(string path)
        {
            if (File.Exists(path))
            {
                string[] file = File.ReadAllLines(path);
                foreach (string line in file)
                {
                    if (line.StartsWith("WallpaperPath="))
                    {
                        wallpath = line.Remove(0, 13);
                    }
                    if (line.StartsWith("OSName="))
                    {
                        osname = line.Remove(0, 7);
                    }
                    if (line.StartsWith("OSVersion="))
                    {
                        osversion = line.Remove(0, 10);
                    }
                    if (line == "VBECanvas")
                    {
                        canvastype = "vbe";
                    }
                    if (line == "SVGACanvas")
                    {
                        canvastype = "svga";
                    }
                    if (line == "VMwareSVGACanvas")
                    {
                        canvastype = "vmsvga";
                    }
                }
            }
            else
            {
                Console.WriteLine("[ERROR] Config file not found");
            }
        }
    }
}