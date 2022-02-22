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

        public static void UpdateArchive(ref CpioArchive Archive, string RootDir, string[] CommandsList)
        {
            var Processed = new List<string>();
            var Filenames = Array.ConvertAll(Archive.Files.ToArray(), F => F.Path);
            foreach(var F in Filenames)
            {
                var LocalPath = Path.Combine(RootDir, F);

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

            foreach (var Cmd in CommandsList)
            {
                var Parts = Cmd.Split(new char[] { ' ' });
                ProcessCommand(Parts, Archive);
            }
        }

        private static UInt32 ConvertMode(string Mode)
        {
            UInt32 ModeX = 0;
            for(int i = 0; i < 3; i++)
            {
                int Offset = i * 3;

                for(int c = 0; c < 3; c++)
                {
                    var C = Mode[Offset + c];

                    if (C == 'r') ModeX |= 4U << ((2 - i) * 4);
                    if (C == 'w') ModeX |= 2U << ((2 - i) * 4);
                    if (C == 'x') ModeX |= 1U << ((2 - i) * 4);
                }
            }
            return ModeX;
        }

        private static string ConvertModeToString(UInt32 Mode)
        {
            var Res = "";
            for(int i = 0; i < 3; i++)
            {
                UInt32 Part = (Mode >> (2 - i) * 4) & 0x7;

                Res += ((Part & 0x04) != 0) ? "r" : "-";
                Res += ((Part & 0x02) != 0) ? "w" : "-";
                Res += ((Part & 0x01) != 0) ? "x" : "-";
            }
            return Res;
        }

        private static void ProcessCommand(string[] Command, CpioArchive Archive)
        {
            if(Command.Length>= 2)
            {
                var Cmd = Command[0];
                var Path = Command[1];
                switch (Cmd)
                {
                    case "rm": 
                        Archive.Delete(Path);
                        Console.WriteLine($"Delete  {Path}");
                        break;
                    case "chmod":
                        if (Command.Length == 3)
                        {
                            var Mode = (Command[2].Length == 9) ? ConvertMode(Command[2]) : Convert.ToUInt32(Command[2], 16) & 0xFFF;
                            Archive.ChMod(Path, Mode);
                            Console.WriteLine($"ChMod    {Path}: {ConvertModeToString(Mode)}");
                        }
                        break;
                }
            }
        }
    }
}
