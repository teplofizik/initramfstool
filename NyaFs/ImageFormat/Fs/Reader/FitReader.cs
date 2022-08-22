using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Reader
{
    public class FitReader : Reader
    {
        bool Loaded = false;

        FlattenedDeviceTree.FlattenedDeviceTree Fit;
        FlattenedDeviceTree.Types.Node RamdiskNode = null;

        public FitReader(string Filename)
        {
            Fit = new NyaFs.FlattenedDeviceTree.Reader.FDTReader(Filename).Read();

            if(Fit.Root.Nodes.Count == 0)
            {
                Log.Error(0, $"Could not load FIT image from file {Filename}");
                return;
            }

            var Configurations = Fit.Get("configurations");
            if(Configurations == null)
            {
                Log.Error(0, $"Invalid FIT image {Filename}: no 'configuration' node.");
                return;
            }
            var DefaultConfig = Configurations.GetStringValue("default");

            if (DefaultConfig == null)
            {
                Log.Error(0, $"Invalid FIT image {Filename}: no 'default' parameter in 'configuration' node.");
                return;
            }

            var Configuration = Fit.Get("configurations/" + DefaultConfig);

            if (Configuration == null)
            {
                Log.Error(0, $"Invalid FIT image {Filename}: no 'configuration/{DefaultConfig}' node.");
                return;
            }

            // kernel fdt ramdisk
            var RamdiskNodeName = Configuration.GetStringValue("ramdisk");
            if (Configuration == null)
            {
                Log.Error(0, $"Invalid FIT image {Filename}: no 'ramdisk' parameter in 'configuration/{DefaultConfig}' node.");
                return;
            }

            RamdiskNode = Fit.Get($"images/{RamdiskNodeName}");
            if (RamdiskNode == null)
            {
                Log.Error(0, $"Invalid FIT image {Filename}: no '{RamdiskNodeName}' node.");
                return;
            }

            Loaded = true;
        }

        /// <summary>
        /// Читаем в файловую систему из cpio-файла
        /// </summary>
        /// <param name="Dst"></param>
        public override void ReadToFs(Filesystem Dst)
        {
            if (!Loaded) return;

            var FsType = RamdiskNode.GetStringValue("type");
            if (FsType == null)
            {
                Log.Error(0, $"Invalid FIT image: no 'type' parameter in loaded ramdisk node.");
                return;
            }

            var Arch = RamdiskNode.GetStringValue("arch");
            if (Arch == null)
            {
                Log.Error(0, $"Invalid FIT image: no 'arch' parameter in loaded ramdisk node.");
                return;
            }

            var Os = RamdiskNode.GetStringValue("os");
            if (Os == null)
            {
                Log.Error(0, $"Invalid FIT image: no 'os' parameter in loaded ramdisk node.");
                return;
            }

            var CompressedData = RamdiskNode.GetValue("data");
            if(CompressedData == null)
            {
                Log.Error(0, $"Invalid FIT image: no 'data' parameter in loaded ramdisk node.");
                return;
            }

            var Compression = RamdiskNode.GetStringValue("compression");
            if (Compression == null)
            {
                Log.Error(0, $"Invalid FIT image: no 'compression' parameter in loaded ramdisk node.");
                return;
            }

            var Data = GetDecompressedData(CompressedData, Compression);

            var FilesystemType = FilesystemDetector.DetectFs(Data);

            Log.Ok(1, $"  Operating System: {Os}");
            Log.Ok(1, $"      Architecture: {Arch}");
            Log.Ok(1, $"       Compression: {Compression}");
            Log.Ok(1, $"              Type: {FsType}");
            Log.Ok(1, $"        Filesystem: {FilesystemDetector.GetFilesystemType(FilesystemType)}");

            if (FilesystemType == Types.FsType.Cpio)
            {
                var Reader = new CpioReader(Data);
                Reader.ReadToFs(Dst);

                Dst.Info.Architecture = GetCPUArchitecture(Arch);
                Dst.Info.OperatingSystem = GetOperatingSystem(Os);
                Dst.Info.Name = null;
                Dst.Info.DataLoadAddress = 0;
                Dst.Info.EntryPointAddress = 0;
                Dst.Info.Type = GetType(FsType);
            }
            //else if (FilesystemType == Types.FsType.Ext4)
            //{
            //    var Reader = new ExtReader(Data);
            //    Reader.ReadToFs(Dst);
            //}
            else
                Log.Error(0, "Unsupported filesystem...");
        }

        Types.CPU GetCPUArchitecture(string Arch)
        {
            switch (Arch)
            {
                case "alpha": return Types.CPU.IH_ARCH_ALPHA;
                case "arm": return Types.CPU.IH_ARCH_ARM;
                case "arm64": return Types.CPU.IH_ARCH_ARM64;
                case "x86": return Types.CPU.IH_ARCH_I386;
                case "ia64": return Types.CPU.IH_ARCH_IA64;
                case "m68k": return Types.CPU.IH_ARCH_M68K;
                case "microblaze": return Types.CPU.IH_ARCH_MICROBLAZE;
                case "mips": return Types.CPU.IH_ARCH_MIPS;
                case "mips64": return Types.CPU.IH_ARCH_MIPS64;
                case "nios": return Types.CPU.IH_ARCH_NIOS;
                case "nios2": return Types.CPU.IH_ARCH_NIOS2;
                case "powerpc":
                case "ppc": return Types.CPU.IH_ARCH_PPC;
                case "s390": return Types.CPU.IH_ARCH_S390;
                case "sh": return Types.CPU.IH_ARCH_SH;
                case "sparc": return Types.CPU.IH_ARCH_SPARC;
                case "sparc64": return Types.CPU.IH_ARCH_SPARC64;
                case "blackfin": return Types.CPU.IH_ARCH_BLACKFIN;
                case "avr32": return Types.CPU.IH_ARCH_AVR32;
                case "nds32": return Types.CPU.IH_ARCH_NDS32;
                case "or1k": return Types.CPU.IH_ARCH_OPENRISC;
                case "sandbox": return Types.CPU.IH_ARCH_SANDBOX;
                case "arc": return Types.CPU.IH_ARCH_ARC;
                case "x86_64": return Types.CPU.IH_ARCH_X86_64;
                case "xtensa": return Types.CPU.IH_ARCH_XTENSA;
                case "riscv": return Types.CPU.IH_ARCH_RISCV;

                default: return Types.CPU.IH_ARCH_INVALID;
            }
        }

        Types.OS GetOperatingSystem(string Os)
        {
            switch (Os)
            {
                case "linux": return ImageFormat.Types.OS.IH_OS_LINUX;
                case "lynxos": return ImageFormat.Types.OS.IH_OS_LYNXOS;
                case "netbsd": return ImageFormat.Types.OS.IH_OS_NETBSD;
                case "ose": return ImageFormat.Types.OS.IH_OS_OSE; // ENEA OSE RTOS
                case "plan9": return ImageFormat.Types.OS.IH_OS_PLAN9;
                case "rtems": return ImageFormat.Types.OS.IH_OS_RTEMS;
                case "tee": return ImageFormat.Types.OS.IH_OS_TEE;
                case "u-boot": return ImageFormat.Types.OS.IH_OS_U_BOOT;
                case "vxworks": return ImageFormat.Types.OS.IH_OS_VXWORKS;
                case "qnx": return ImageFormat.Types.OS.IH_OS_QNX;
                case "integrity": return ImageFormat.Types.OS.IH_OS_INTEGRITY;
                case "4_4bsd": return ImageFormat.Types.OS.IH_OS_4_4BSD;
                case "dell": return ImageFormat.Types.OS.IH_OS_DELL;
                case "esix": return ImageFormat.Types.OS.IH_OS_ESIX;
                case "freebsd": return ImageFormat.Types.OS.IH_OS_FREEBSD;
                case "irix": return ImageFormat.Types.OS.IH_OS_IRIX;
                case "ncr": return ImageFormat.Types.OS.IH_OS_NCR;
                case "openbsd": return ImageFormat.Types.OS.IH_OS_OPENBSD;
                case "psos": return ImageFormat.Types.OS.IH_OS_PSOS;
                case "sco": return ImageFormat.Types.OS.IH_OS_SCO;
                case "solaris": return ImageFormat.Types.OS.IH_OS_SOLARIS;
                case "svr4": return ImageFormat.Types.OS.IH_OS_SVR4;
                case "openrtos": return ImageFormat.Types.OS.IH_OS_OPENRTOS;
                case "opensbi": return ImageFormat.Types.OS.IH_OS_OPENSBI;
                case "efi": return ImageFormat.Types.OS.IH_OS_EFI;
                //case "artos": return ImageFormat.Types.OS.IH_OS_ARTOS;
                //case "unity": return ImageFormat.Types.OS.IH_OS_UNITY;
                //case "armtrusted": return ImageFormat.Types.OS.IH_OS_ARM_TRUSTED_FIRMWARE;
                default: return Types.OS.IH_OS_INVALID;
            }
        }

        ImageFormat.Types.ImageType GetType(string Type)
        {
            switch(Type)
            {
                case "kernel": return ImageFormat.Types.ImageType.IH_TYPE_KERNEL;
                case "flat_dt": return ImageFormat.Types.ImageType.IH_TYPE_FLATDT;
                case "ramdisk": return ImageFormat.Types.ImageType.IH_TYPE_RAMDISK;
                case "aisimage": return ImageFormat.Types.ImageType.IH_TYPE_AISIMAGE;
                case "filesystem": return ImageFormat.Types.ImageType.IH_TYPE_FILESYSTEM;
                case "firmware": return ImageFormat.Types.ImageType.IH_TYPE_FIRMWARE;
                case "gpimage": return ImageFormat.Types.ImageType.IH_TYPE_GPIMAGE;
                case "kernel_noload": return ImageFormat.Types.ImageType.IH_TYPE_KERNEL_NOLOAD;
                case "kwbimage": return ImageFormat.Types.ImageType.IH_TYPE_KWBIMAGE;
                case "imximage": return ImageFormat.Types.ImageType.IH_TYPE_IMXIMAGE;
                case "imx8image": return ImageFormat.Types.ImageType.IH_TYPE_IMX8IMAGE;
                case "imx8mimage": return ImageFormat.Types.ImageType.IH_TYPE_IMX8MIMAGE;
                case "multi": return ImageFormat.Types.ImageType.IH_TYPE_MULTI;
                case "omapimage": return ImageFormat.Types.ImageType.IH_TYPE_OMAPIMAGE;
                case "pblimage": return ImageFormat.Types.ImageType.IH_TYPE_PBLIMAGE;
                case "script": return ImageFormat.Types.ImageType.IH_TYPE_SCRIPT;
                case "socfpgaimage": return ImageFormat.Types.ImageType.IH_TYPE_SOCFPGAIMAGE;
                case "socfpgaimage_v1": return ImageFormat.Types.ImageType.IH_TYPE_SOCFPGAIMAGE_V1;
                case "standalone": return ImageFormat.Types.ImageType.IH_TYPE_STANDALONE;
                case "ublimage": return ImageFormat.Types.ImageType.IH_TYPE_UBLIMAGE;
                case "mxsimage": return ImageFormat.Types.ImageType.IH_TYPE_MXSIMAGE;
                case "atmelimage": return ImageFormat.Types.ImageType.IH_TYPE_ATMELIMAGE;
                case "x86_setup": return ImageFormat.Types.ImageType.IH_TYPE_X86_SETUP;
                case "lpc32xximage": return ImageFormat.Types.ImageType.IH_TYPE_LPC32XXIMAGE;
                case "rkimage": return ImageFormat.Types.ImageType.IH_TYPE_RKIMAGE;
                case "rksd": return ImageFormat.Types.ImageType.IH_TYPE_RKSD;
                case "rkspi": return ImageFormat.Types.ImageType.IH_TYPE_RKSPI;
                case "vybridimage": return ImageFormat.Types.ImageType.IH_TYPE_VYBRIDIMAGE;
                case "zynqimage": return ImageFormat.Types.ImageType.IH_TYPE_ZYNQIMAGE;
                case "zynqmpimage": return ImageFormat.Types.ImageType.IH_TYPE_ZYNQMPIMAGE;
                case "zynqmpbif": return ImageFormat.Types.ImageType.IH_TYPE_ZYNQMPBIF;
                case "fpga": return ImageFormat.Types.ImageType.IH_TYPE_FPGA;
                case "tee": return ImageFormat.Types.ImageType.IH_TYPE_TEE;
                case "firmware_ivt": return ImageFormat.Types.ImageType.IH_TYPE_FIRMWARE_IVT;
                case "pmmc": return ImageFormat.Types.ImageType.IH_TYPE_PMMC;
                case "stm32image": return ImageFormat.Types.ImageType.IH_TYPE_STM32IMAGE;
                case "mtk_image": return ImageFormat.Types.ImageType.IH_TYPE_MTKIMAGE;
                case "copro": return ImageFormat.Types.ImageType.IH_TYPE_COPRO;
                case "sunxi_egon": return ImageFormat.Types.ImageType.IH_TYPE_SUNXI_EGON;
                default: return ImageFormat.Types.ImageType.IH_TYPE_INVALID;
            }
        }

        byte[] GetDecompressedData(byte[] Source, string Compression)
        {
            switch (Compression)
            {
                case "gzip": return Compressors.Gzip.Decompress(Source);
                case "none": return Source;
                default:
                    Log.Error(0, $"Unsupported compression type: {Compression}");
                    throw new ArgumentException($"Unsupported compression type: {Compression}");
            }
        }
    }
}
