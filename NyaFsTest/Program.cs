using System;

namespace NyaFsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadSaveFit();
        }

        static void LoadSaveFdt()
        {
            var fdt = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d-bitmainer.dtb").Read();

            if (fdt != null)
            {
                Console.WriteLine("FDT ok");

                var data = new NyaFs.FlattenedDeviceTree.Writer.FDTWriter(fdt);
                System.IO.File.WriteAllBytes("amlogic-a113d-bitmainer-saved.dtb", data.GetBinary());

                var readout = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d-bitmainer-saved.dtb").Read();

            }
        }

        static void LoadSaveFit()
        {
            var fdt = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d.fit").Read();

            if (fdt != null)
            {
                Console.WriteLine("FDT ok");

                var data = new NyaFs.FlattenedDeviceTree.Writer.FDTWriter(fdt);
                System.IO.File.WriteAllBytes("amlogic-a113d-saved.fit", data.GetBinary());

                var readout = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d-saved.fit").Read();
            }
        }

        static void LoadFdt()
        {
            var fdt = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d.fit").Read();

            if (fdt != null)
            {
                Console.WriteLine("FIT ok");
            }
        }

        static void LoadFit()
        {
            var fit = new NyaFs.FlattenedDeviceTree.Reader.FDTReader("amlogic-a113d.fit").Read();

            if (fit != null)
            {
                Console.WriteLine("FIT ok");
            }
        }
    }
}
