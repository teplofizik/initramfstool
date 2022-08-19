using System;
using System.Diagnostics;
using System.IO;

namespace BmuTool
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessBmu();
            //ProcessUbootDump();
        }

        static void ProcessBmu()
        {
            var Fn = "Antminer-S19j-Pro-A-merge-release-20220329100250-6in1";
            //var Fn = "Antminer_S19j_Pro_merge_release_20211021062136_6IN1_download12_01";
            //var Fn = "Antminer-S19j-Pro-merge-release-20220329103153-9in1";
            var Data = System.IO.File.ReadAllBytes($"F:\\Проекты\\Асик конфиг\\AM1130\\bmu\\{Fn}.bmu");
            var Parser = new Bmu.BmuParser();

            Parser.Parse(Data, Fn);
        }

        static void ProcessUbootDump()
        {
            var Dump = File.ReadAllLines("F:\\Проекты\\Асик конфиг\\AM1130\\dump\\ramfs.udump");
            var Parser = new UbootDumpParser();

            var Bytes = Parser.ConvertToBytes(Dump);
            Debug.WriteLine($"Dumped: {Bytes.Length}");
            File.WriteAllBytes("F:\\Проекты\\Асик конфиг\\AM1130\\dump\\ramfs.bin", Bytes);
        }
    }
}
