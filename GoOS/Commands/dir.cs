using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoOS.Commands
{
    internal class Dir
    {
        //GoOS Core
        public static void print(string str)
        {
            Console.WriteLine(str);
        }
        public static void log(System.ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }
        public static void write(string str)
        {
            Console.Write(str);
        }
        public static void textcolour(System.ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }
        public static void highlightcolour(System.ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }
        public static void sleep(int time)
        {
            Thread.Sleep(time);
        }
        public static void Run()
        {
            int filecount = 0;
            int foldercount = 0;
            string cdir3002 = Directory.GetCurrentDirectory();
            string cdir3003 = @"0:\";
            if (cdir3002.Contains(@"0:\\"))
            {
                cdir3003 = cdir3002.Replace(@"0:\\", @"0:\");
            }
            try
            {
                var directory_list = Directory.GetFiles(cdir3003);
                var directory2_list = Directory.GetDirectories(cdir3003);
                log(ConsoleColor.Gray, "\nDirectory listing at " + cdir3003 + "\n");
                foreach (var directory in directory2_list)
                {
                    log(ConsoleColor.Gray, "<Dir> " + directory);
                    foldercount++;
                }
                foreach (var file in directory_list)
                {
                    log(ConsoleColor.Gray, "<File> " + file);
                    filecount++;
                }
                log(ConsoleColor.Gray, $"\nListed {filecount} files and {foldercount} folders in this directory.\n");
            }
            catch (Exception e)
            {
                log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
            }
        }
    }
}
