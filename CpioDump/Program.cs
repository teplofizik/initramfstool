using CpioDump.Dumper;
using System;
using CpioLib.IO;

namespace CpioDump
{
    class Program
    {
        static void Main(string[] args)
        {
            // SD LOADER: C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
            // 
            var Packed = CpioParser.Load(@"C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.packed.cpio");
            var Original = CpioParser.Load(@"C:\Users\Professional\Documents\antminer\encruption\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio");

            foreach(var F in Original.Files)
            {
                if(!Packed.Exists(F.Path))
                {
                    Console.WriteLine($"{F.Path} not found");
                }
            }
            //Dumper.Dump();
        }
    }
}
