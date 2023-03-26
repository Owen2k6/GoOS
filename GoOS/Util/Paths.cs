/*
 * Author: Jspa2 <github.com/Jspa2>
 * Summary: Utilities for working with paths.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GoOS.Util
{
    public static class Paths
    {
        private static void ApplySyntax(List<string> split)
        {
            for (int i = 0; i < split.Count; i++)
            {
                if (split[i] == ".")
                {
                    split.RemoveAt(i);
                    i--;
                }
                else if (split[i] == "..")
                {
                    if (i < 2)
                    {
                        split.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        split.RemoveRange(i - 1, 2);
                        i -= 2;
                    }
                }
            }
        }

        private static void RemoveEmpty(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].Length == 0)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public static string JoinPaths(string front, string back)
        {
            front = RemoveDuplicateDirectorySeparators(front).Replace('/', '\\').Trim();
            back = RemoveDuplicateDirectorySeparators(back).Replace('/', '\\').Trim();

            List<string> frontParts = new List<string>(front.Split('\\'));
            List<string> backParts = new List<string>(back.Split('\\'));

            RemoveEmpty(frontParts);
            RemoveEmpty(backParts);

            List<string> parts;

            // Check if the back path is a rooted path.
            if (backParts[0].EndsWith(':') || backParts[0] == "~" || back[0] == '\\')
            {
                // If the back path starts with a path separator, we must assume the drive from the front path.
                if (back[0] == '\\')
                {
                    if (!frontParts[0].EndsWith(':'))
                    {
                        // Ambiguous path.
                        throw new InvalidOperationException("Unable to join paths.");
                    }

                    // Assume the drive from the front path.
                    parts = new List<string>() { frontParts[0] };
                    parts.AddRange(backParts);
                }
                else
                {
                    parts = backParts;
                }
            }
            else
            {
                // The back path is a relative path.
                parts = frontParts;

                parts.AddRange(backParts);
            }

            ApplySyntax(parts);

            var result = new StringBuilder();
            for (int i = 0; i < parts.Count; i++)
            {
                result.Append(parts[i]);
                if (parts.Count == 1 | i != parts.Count - 1)
                {
                    result.Append('\\');
                }
            }

            return RemoveDuplicateDirectorySeparators(result.ToString());
        }

        public static string RemoveDuplicateDirectorySeparators(string path)
        {
            var result = new StringBuilder();
            bool lastSeparator = false;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '\\')
                {
                    if (lastSeparator)
                    {
                        continue;
                    }
                    lastSeparator = true;
                }
                else
                {
                    lastSeparator = false;
                }
                result.Append(path[i]);
            }
            return result.ToString();
        }

        public static string SanitisePath(string path)
        {
            return RemoveDuplicateDirectorySeparators(Path.GetFullPath(path).Replace('/', '\\').ToLower().Trim());
        }
    }
}