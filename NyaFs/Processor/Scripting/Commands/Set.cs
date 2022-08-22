using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor.Scripting.Commands
{
    public class Set : ScriptStepGenerator
    {
        public Set() : base("set")
        {

            AddConfig(new ScriptArgsConfig(0, new ScriptArgsParam[] {
                    new Params.EnumScriptArgsParam("type", new string[] { "kernel", "devtree", "ramfs", "all" }),
                    new Params.EnumScriptArgsParam("param", new string[] { "os", "arch" }),
                    new Params.StringScriptArgsParam("value")
                }));
        }

        public override ScriptStep Get(ScriptArgs Args)
        {
            var A = Args.RawArgs;

            return new SetScriptStep(A[0], A[1], A[2]);
        }

        public class SetScriptStep : ScriptStep
        {
            string Type;
            string Param;
            string Value;

            public SetScriptStep(string Type, string Param, string Value) : base("set")
            {
                this.Type = Type;
                this.Param = Param;
                this.Value = Value;
            }

            public override ScriptStepResult Exec(ImageProcessor Processor)
            {
                List<ImageFormat.Types.ImageInfo> Info = new List<ImageFormat.Types.ImageInfo>();

                switch (Type)
                {
                    case "ramfs":
                        {
                            var Fs = Processor.GetFs();
                            if (Fs != null)
                                Info.Add(Fs.Info);
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, "Filesystem is not loaded");
                        }
                        break;
                    case "devtree":
                        {
                            var Dt = Processor.GetDevTree();
                            if (Dt != null)
                                Info.Add(Dt.Info);
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, "Device tree is not loaded");
                        }
                        break;
                    case "kernel":
                        {
                            var Kernel = Processor.GetKernel();
                            if (Kernel != null)
                                Info.Add(Kernel.Info);
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, "Kernel is not loaded");
                        }
                        break;
                    case "all":
                        {

                        }
                        break;
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown image type!");
                }

                // Ok. Image info is selected. Now update it.
                switch(Param)
                {
                    case "os":
                        {
                            var OS = ParseOS(Value);
                            if (OS != ImageFormat.Types.OS.IH_OS_INVALID)
                            {
                                Array.ForEach(Info.ToArray(), I => I.OperatingSystem = OS);
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Set OS ok: {Value}!");

                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown OS: {Value}!");

                        }
                    case "arch":
                        {
                            var Arch = ParseArch(Value);
                            if (Arch != ImageFormat.Types.CPU.IH_ARCH_INVALID)
                            {
                                Array.ForEach(Info.ToArray(), I => I.Architecture = Arch);
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Set architecture ok: {Value}!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown architecture: {Value}!");

                        }
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown parameter {Param}!");
                }
            }

            ImageFormat.Types.CPU ParseArch(string Arch)
            {
                switch (Arch.ToLower())
                {
                    case "alpha": return ImageFormat.Types.CPU.IH_ARCH_ALPHA;
                    case "arm":  return ImageFormat.Types.CPU.IH_ARCH_ARM; // ARM
                    case "x86":
                    case "i386": return ImageFormat.Types.CPU.IH_ARCH_I386; // Intel x86
                    case "ia64": return ImageFormat.Types.CPU.IH_ARCH_IA64; // IA64
                    case "mips": return ImageFormat.Types.CPU.IH_ARCH_MIPS; // MIPS
                    case "mips64": return ImageFormat.Types.CPU.IH_ARCH_MIPS64; // MIPS 64 Bit
                    case "ppc": return ImageFormat.Types.CPU.IH_ARCH_PPC; // PowerPC
                    case "s390": return ImageFormat.Types.CPU.IH_ARCH_S390; // IBM S390
                    case "superh": return ImageFormat.Types.CPU.IH_ARCH_SH; // SuperH
                    case "sparc": return ImageFormat.Types.CPU.IH_ARCH_SPARC; // Sparc
                    case "sparc64": return ImageFormat.Types.CPU.IH_ARCH_SPARC64; // Sparc 64 Bit
                    case "m68k": return ImageFormat.Types.CPU.IH_ARCH_M68K; // M68K
                    case "nios": return ImageFormat.Types.CPU.IH_ARCH_NIOS; // Nios-32
                    case "microblaze": return ImageFormat.Types.CPU.IH_ARCH_MICROBLAZE; // MicroBlaze
                    case "nios2": return ImageFormat.Types.CPU.IH_ARCH_NIOS2; // Nios-II
                    case "blackfin": return ImageFormat.Types.CPU.IH_ARCH_BLACKFIN; // Blackfin
                    case "avr32": return ImageFormat.Types.CPU.IH_ARCH_AVR32; // AVR32
                    case "st200": return ImageFormat.Types.CPU.IH_ARCH_ST200; // STMicroelectronics ST200 
                    case "sandbox": return ImageFormat.Types.CPU.IH_ARCH_SANDBOX; // Sandbox architecture (test only)
                    case "nds32": return ImageFormat.Types.CPU.IH_ARCH_NDS32; // ANDES Technology - NDS32
                    case "openrisc": return ImageFormat.Types.CPU.IH_ARCH_OPENRISC; // OpenRISC 1000 
                    case "aarch64":
                    case "arm64": return ImageFormat.Types.CPU.IH_ARCH_ARM64; // ARM64
                    case "arc": return ImageFormat.Types.CPU.IH_ARCH_ARC; // Synopsys DesignWare ARC
                    case "x86_64": return ImageFormat.Types.CPU.IH_ARCH_X86_64; // AMD x86_64, Intel and Via
                    case "xtensa": return ImageFormat.Types.CPU.IH_ARCH_XTENSA; // Xtensa
                    case "riscv": return ImageFormat.Types.CPU.IH_ARCH_RISCV; // RISC-V
                    default: return ImageFormat.Types.CPU.IH_ARCH_INVALID;
                }
            }

            ImageFormat.Types.OS ParseOS(string OS)
            {
                switch (OS.ToLower())
                {
                    case "openbsd": return ImageFormat.Types.OS.IH_OS_OPENBSD;
                    case "netbsd": return ImageFormat.Types.OS.IH_OS_NETBSD;
                    case "freebsd": return ImageFormat.Types.OS.IH_OS_FREEBSD;
                    case "4.4bsd": return ImageFormat.Types.OS.IH_OS_4_4BSD;
                    case "linux": return ImageFormat.Types.OS.IH_OS_LINUX;
                    case "svr4": return ImageFormat.Types.OS.IH_OS_SVR4;
                    case "esix": return ImageFormat.Types.OS.IH_OS_ESIX;
                    case "solaris": return ImageFormat.Types.OS.IH_OS_SOLARIS;
                    case "irix": return ImageFormat.Types.OS.IH_OS_IRIX;
                    case "sco": return ImageFormat.Types.OS.IH_OS_SCO;
                    case "dell": return ImageFormat.Types.OS.IH_OS_DELL;
                    case "ncr": return ImageFormat.Types.OS.IH_OS_NCR;
                    case "luyx": return ImageFormat.Types.OS.IH_OS_LYNXOS;
                    case "vxworks": return ImageFormat.Types.OS.IH_OS_VXWORKS;
                    case "psos": return ImageFormat.Types.OS.IH_OS_PSOS;
                    case "qnx": return ImageFormat.Types.OS.IH_OS_QNX;
                    case "uboot": return ImageFormat.Types.OS.IH_OS_U_BOOT;
                    case "rtems": return ImageFormat.Types.OS.IH_OS_RTEMS;
                    case "artos": return ImageFormat.Types.OS.IH_OS_ARTOS;
                    case "unity": return ImageFormat.Types.OS.IH_OS_UNITY;
                    case "integrity": return ImageFormat.Types.OS.IH_OS_INTEGRITY;
                    case "ose": return ImageFormat.Types.OS.IH_OS_OSE;
                    case "plan9": return ImageFormat.Types.OS.IH_OS_PLAN9;
                    case "openrtos": return ImageFormat.Types.OS.IH_OS_OPENRTOS;
                    case "armtrusted": return ImageFormat.Types.OS.IH_OS_ARM_TRUSTED_FIRMWARE;
                    case "tee": return ImageFormat.Types.OS.IH_OS_TEE;
                    case "opensbi": return ImageFormat.Types.OS.IH_OS_OPENSBI;
                    case "efi": return ImageFormat.Types.OS.IH_OS_EFI;
                    default: return ImageFormat.Types.OS.IH_OS_INVALID;
                }
            }
        }
    }
}
