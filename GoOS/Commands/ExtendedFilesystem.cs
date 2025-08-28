using System.IO;

namespace GoOS.Commands;

public class ExtendedFilesystem
{
    public static void CopyFile(string from, string to)
    {
        if (from.Contains("\\"))
        {
            //Console.WriteLine("1");
            var whatToRemove = from.Substring(from.LastIndexOf("\\"));

            var FullName = from.Replace(whatToRemove, "");

            var name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllBytes(from);
            File.Create(to + FullName);
            File.WriteAllBytes(to + FullName, Contents);
        }
        else
        {
            //Console.WriteLine("2");
            var FullName = from;

            var name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllBytes(from);
            File.Create(to + FullName);
            File.WriteAllBytes(to + FullName, Contents);
        }
    }

    public static void MoveFile(string from, string to)
    {
        if (from.Contains("\\"))
        {
            var whatToRemove = from.Substring(from.LastIndexOf("\\"));

            var FullName = from.Replace(whatToRemove, "");

            var name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllBytes(from);
            File.Create(to + FullName);
            File.WriteAllBytes(to + FullName, Contents);
            File.Delete(from);
        }
        else
        {
            var FullName = from;

            var name = FullName.Substring(FullName.IndexOf("."));

            var Contents = File.ReadAllBytes(from);
            File.Create(to + FullName);
            File.WriteAllBytes(to + FullName, Contents);
            File.Delete(from);
        }
    }
}