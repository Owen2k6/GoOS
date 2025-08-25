using System;
using System.IO;
using GoOS.GUI;

namespace GoOS.Apps
{
    public static class GiffRunner
    {
        /// <summary>Executes a .giff file from disk. Shows a Dialogue on errors.</summary>
        public static void RunFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Dialogue.Show("Giff", "No file specified.");
                return;
            }

            try
            {
                if (!File.Exists(path))
                {
                    Dialogue.Show("Giff", $"File not found:\n{path}");
                    return;
                }

                string script = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(script))
                {
                    Dialogue.Show("Giff", "File is empty.");
                    return;
                }

                var windows = Giff.Giff.Run(script);
                if (windows == null || windows.Count == 0)
                    Dialogue.Show("Giff", "Script executed, but no windows were created.");
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Failed to execute script:\n" + ex);
            }
        }
    }
}
