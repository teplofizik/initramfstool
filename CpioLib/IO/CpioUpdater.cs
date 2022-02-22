using CpioLib.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CpioLib.IO
{
    public static class CpioUpdater
    {
        public static void Info(ref CpioArchive Archive)
        {
            foreach (var F in Archive.Files)
            {
                Console.WriteLine($"{F.Path}: m:{F.Mode:x02} in:{F.INode} links:{F.NumLink} maj:{F.Major} min:{F.Minor} rmaj:{F.RMajor} rmin:{F.RMinor}");
            }
        }

        public static void UpdateArchive(ref CpioArchive Archive, string RootDir, string[] DeleteList)
        {
            var Processed = new List<string>();
            var Filenames = Array.ConvertAll(Archive.Files.ToArray(), F => F.Path);
            foreach(var F in Filenames)
            {
                var LocalPath = Path.Combine(RootDir, F);

                if(DeleteList.Contains(F))
                {
                    Console.WriteLine($"Delete  {F}");

                    Archive.Delete(F);
                    Processed.Add(F);
                }

                if(File.Exists(LocalPath))
                {
                    Console.WriteLine($"Update  {F}");

                    Archive.UpdateFile(F, LocalPath);
                    Processed.Add(F);
                }
            }

            var UpdateDirs = Directory.GetDirectories(RootDir, "*", SearchOption.AllDirectories);
            foreach (var F in UpdateDirs)
            {
                // ./root/etc\init.d\pgnand.sh
                var ConvertedF = F.Substring(RootDir.Length).Replace('\\', '/');

                if (!Archive.Exists(ConvertedF))
                {
                    Archive.AddDir(ConvertedF, F);
                    Console.WriteLine($"Add dir {ConvertedF}");

                    Processed.Add(ConvertedF);
                }
            }

            var UpdateFiles = Directory.GetFiles(RootDir, "*", SearchOption.AllDirectories);
            foreach (var F in UpdateFiles)
            {
                // ./root/etc\init.d\pgnand.sh
                var ConvertedF = F.Substring(RootDir.Length).Replace('\\', '/');

                if (!Processed.Contains(ConvertedF))
                {
                    var Content = File.ReadAllBytes(F);

                    if (!Archive.Exists(ConvertedF))
                    {
                        Archive.AddFile(ConvertedF, F);
                        Console.WriteLine($"Add     {ConvertedF}");
                        Processed.Add(ConvertedF);
                    }
                }
            }
        }
    }
}
