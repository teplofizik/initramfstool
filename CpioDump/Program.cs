using CpioDump.Dumper;
using System;
using CpioLib.IO;
using RamFsLib.Types;
using System.IO;
using System.Collections.Generic;

namespace CpioDump
{
    class Program
    {
        static void Main(string[] args)
        {
            var Args = new ArgParser(args);

            var Cpio = Args.GetArg("cpio");
            if (Cpio != null)
                ModifyCpio(Args);
            else
            {
                var RamFsFile = Args.GetArg("ramfs");
                if (RamFsFile != null)
                    ModifyInitramfs(Args);
            }
        }

        static void ModifyCpio(ArgParser Args)
        {
            var Cpio = Args.GetArg("cpio");
            var Root = Args.GetArg("root");
            var Delete = Args.GetArg("delete");
            var Out = Args.GetArg("out");

            if (Cpio != null)
            {
                if (!File.Exists(Cpio))
                {
                    Console.WriteLine($"CPIO file not found: {Cpio}");
                }
                else
                {
                    var Original = CpioParser.Load(Cpio);

                    if (Root != null)
                    {
                        var DeleteList = GetDeleteList(Delete);
                        // SD LOADER: C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
                        //var Packed = CpioParser.Load(@"C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.packed.cpio");

                        CpioUpdater.UpdateArchive(ref Original, Root, DeleteList);

                        if (Out != null)
                            CpioPacker.Save(Original, Out);
                    }
                    else
                    {
                        var Extract = Args.GetArg("extract");
                        if (Extract != null)
                        {
                            CpioExtractor.Extract(Original, Extract);
                        }
                        else
                        {
                            CpioUpdater.Info(ref Original);
                        }
                    }
                }
            }
        }

        static void ModifyInitramfs(ArgParser Args)
        {
            var RamFsFile = Args.GetArg("ramfs");
            var Root = Args.GetArg("root");
            var Delete = Args.GetArg("delete");
            var Out = Args.GetArg("out");

            if (RamFsFile != null)
            {
                if (!File.Exists(RamFsFile))
                {
                    Console.WriteLine($"RamFS file not found: {RamFsFile}");
                }
                else
                {
                    var Fs = new InitramFsFile(RamFsFile);
                    var CpioRaw = Fs.UnpackedData;
                    var Original = CpioParser.Load(CpioRaw);

                    if (Root != null)
                    {
                        var DeleteList = GetDeleteList(Delete);

                        CpioUpdater.UpdateArchive(ref Original, Root, DeleteList);

                        if (Out != null)
                        {
                            var UnpackedDst = CpioPacker.GetRawData(Original);

                            var NewFs = Fs.GetModified(UnpackedDst);
                            File.WriteAllBytes(Out, NewFs.getPacket());
                        }
                    }
                    else
                    {
                        var Extract = Args.GetArg("extract");
                        if (Extract != null)
                        {
                            CpioExtractor.Extract(Original, Extract);
                        }
                        else
                        {
                            CpioUpdater.Info(ref Original);
                        }
                    }
                }
            }
        }

        static string[] GetDeleteList(string Filename)
        {
            if ((Filename != null) && File.Exists(Filename))
                return File.ReadAllLines(Filename);
            else
                return new string[] { };
        }
    }
}
