using CpioDump.Dumper;
using System;
using CpioLib.IO;

namespace CpioDump
{
    class Program
    {
        static void Main(string[] args)
        {
            var Args = new ArgParser(args);

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
    }
}
