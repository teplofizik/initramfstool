using CpioLib.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CpioLib.IO
{
    public static class CpioUpdater
    {
        public static void UpdateArchive(ref CpioArchive Archive, string RootDir)
        {
            var Filenames = Array.ConvertAll(Archive.Files.ToArray(), F => F.Path);
            foreach(var F in Filenames)
            {
                var LocalPath = Path.Combine(RootDir, F);

                if(File.Exists(LocalPath))
                {
                    Console.WriteLine($"Update {F}");

                    Archive.UpdateFile(F, LocalPath);
                }
            }
        }
    }
}
