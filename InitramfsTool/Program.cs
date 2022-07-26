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
            var Cmds = Args.GetArg("commands");

            if (Cpio != null)
            {
                if (!File.Exists(Cpio))
                {
                    Console.WriteLine($"CPIO file not found: {Cpio}");
                }
                else
                {
                    var Original = CpioParser.Load(Cpio);

                    if ((Root != null) || (Cmds != null))
                    {
                        var Gzip = Args.GetArg("gzip");
                        var Out = Args.GetArg("out");

                        if ((Gzip != null) || (Out != null))
                        {
                            CpioUpdater.UpdateArchive(ref Original, Root, Cmds);


                            if (Gzip != null)
                                CpioPacker.SaveGz(Original, Gzip);

                            if (Out != null)
                                CpioPacker.Save(Original, Out);
                        }

                        // SD LOADER: C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
                        //var Packed = CpioParser.Load(@"C:\Users\Professional\Documents\antminer\cpio\Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.packed.cpio");

                    }
                    else
                    {
                        var Extract = Args.GetArg("extract");
                        if (Extract != null)
                        {
                            CpioExtractor.Extract(Original, Extract);

                            var BuildScript = Args.GetArg("buildscript");
                            if (BuildScript != null)
                            {
                                CpioExtractor.GenerateScript(Original, Extract, BuildScript);
                            }
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
            var Cmds = Args.GetArg("commands");
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

                    var Unpack = Args.GetArg("unpack");
                    if(Unpack != null)
                    {
                        File.WriteAllBytes(Unpack, CpioRaw);
                    }
                    var Repack = Args.GetArg("repack");
                    if (Repack != null)
                    {
                        if (File.Exists(Repack))
                        {
                            var NewData = File.ReadAllBytes(Repack);

                            if (Out != null)
                            {
                                var NewFs = Fs.GetModified(NewData);
                                File.WriteAllBytes(Out, NewFs.getPacket());
                            }
                        }
                        else
                            Console.WriteLine("File to repack not exists");
                    }
                    else if ((Root != null) || (Cmds != null))
                    {
                        CpioUpdater.UpdateArchive(ref Original, Root, Cmds);

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

                            var BuildScript = Args.GetArg("buildscript");
                            if (BuildScript != null)
                            {
                                CpioExtractor.GenerateScript(Original, Extract, BuildScript);
                            }
                        }
                        else
                        {
                            CpioUpdater.Info(ref Original);
                        }
                    }
                }
            }
        }
    }
}
