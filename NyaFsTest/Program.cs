using System;

namespace NyaFsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestImportRamFs();
        }

        static void TestImportNative()
        {
            var Dir = "F:\\Проекты\\Асик конфиг\\cpiotools\\InitramfsTool\\bin\\Debug\\netcoreapp3.1\\a113d\\full";

            var Fs = new NyaFs.ImageFormat.Fs.Filesystem();
            var Importer = new NyaFs.ImageFormat.Fs.Reader.NativeReader(Dir, 0, 0, 0x744, 0x755);
            Importer.ReadToFs(Fs);

            Fs.Dump();
        }

        static void TestImportCpio()
        {
            var Fn = "F:\\Проекты\\Асик конфиг\\cpiotools\\InitramfsTool\\bin\\Debug\\netcoreapp3.1\\a113d\\s19xp_ramfs.cpio";

            var Fs = new NyaFs.ImageFormat.Fs.Filesystem();
            var Importer = new NyaFs.ImageFormat.Fs.Reader.CpioReader(Fn);
            Importer.ReadToFs(Fs);

            Fs.Dump();
        }

        static void TestImportRamFs()
        {
            var Fn = "F:\\Проекты\\Асик конфиг\\cpiotools\\InitramfsTool\\bin\\Debug\\netcoreapp3.1\\initramfs.bin.SD";

            var Fs = new NyaFs.ImageFormat.Fs.Filesystem();
            var Importer = new NyaFs.ImageFormat.Fs.Reader.LegacyFsReader(Fn);
            Importer.ReadToFs(Fs);

            Fs.Dump();
        }
    }
}
