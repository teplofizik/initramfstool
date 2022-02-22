using CpioDump.Dumper;
using System;
using CpioLib.IO;
using RamFsLib.Types;
using System.IO;

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
            var Out = Args.GetArg("out");

            if ((Cpio != null) && (Root != null))
            {
                // SD LOADER: C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
                //var Packed = CpioParser.Load(@"C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.packed.cpio");
                var Original = CpioParser.Load(Cpio);

                CpioUpdater.UpdateArchive(ref Original, Root);

                if (Out != null)
                    CpioPacker.Save(Original, Out);
            }
        }

        static void ModifyInitramfs(ArgParser Args)
        {
            var RamFsFile = Args.GetArg("ramfs");
            var Root = Args.GetArg("root");
            var Out = Args.GetArg("out");

            if (RamFsFile != null)
            {
                var Fs = new InitramFsFile(RamFsFile);

                var CpioRaw = Fs.UnpackedData;
                var Original = CpioParser.Load(CpioRaw);

                CpioUpdater.UpdateArchive(ref Original, Root);

                if (Out != null)
                {
                    var UnpackedDst = CpioPacker.GetRawData(Original);

                    var NewFs = Fs.GetModified(UnpackedDst);
                    File.WriteAllBytes(Out, NewFs.getPacket());
                }
            }
        }
    }
}
