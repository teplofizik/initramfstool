using System;

namespace NyaFsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadFit();
        }

        static void LoadFdt()
        {
            var fdt = new NyaFs.FlattenedDeviceTree.FlattenedDeviceTree("amlogic-a113d-bitmainer.dtb");

            if(fdt.Correct)
            {
                Console.WriteLine("FDT ok");

                var Root = fdt.Read();
            }
        }

        static void LoadFit()
        {
            var fit = new NyaFs.FlattenedDeviceTree.FlattenedDeviceTree("amlogic-a113d.fit");

            if (fit.Correct)
            {
                Console.WriteLine("FIT ok");

                var Root = fit.Read();
            }
        }
    }
}
