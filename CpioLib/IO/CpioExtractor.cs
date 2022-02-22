using CpioLib.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CpioLib.IO
{
    public static class CpioExtractor
    {
        public static void Extract(CpioArchive Archive, string Dir)
        {
            if(!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            
            foreach(var F in Archive.Files)
            {
                switch(F.FileType)
                {
                    case CpioModeFileType.C_ISDIR: // Dir
                        var D = Path.Combine(Dir, F.Path);
                        if (!Directory.Exists(D))
                            Directory.CreateDirectory(D);

                        Console.WriteLine($"D {F.Path}");
                        break;
                    case CpioModeFileType.C_ISREG: // File
                        var FN = Path.Combine(Dir, F.Path);
                        var Data = F.Content;

                        Console.WriteLine($"F {F.Path}");
                        File.WriteAllBytes(FN, Data);
                        break;
                    default:
                        // Console.WriteLine($"{F.Path}: {GetFileType(F.FileType)}");
                        break;
                }
            }
        }

        private static string GetFileType(CpioModeFileType Type)
        {
            switch(Type)
            {
                case CpioModeFileType.C_ISBLK: return "BLK";
                case CpioModeFileType.C_ISCHR: return "CHR";
                case CpioModeFileType.C_ISCTG: return "CTG";
                case CpioModeFileType.C_ISDIR: return "DIR";
                case CpioModeFileType.C_ISFIFO: return "FIFO";
                case CpioModeFileType.C_ISLNK: return "LNK";
                case CpioModeFileType.C_ISREG: return "REG";
                case CpioModeFileType.C_ISSOCK: return "SOCK";
                default: return $"{Type}";
            }
        }
    }
}
