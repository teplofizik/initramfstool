﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor.Scripting.Commands
{
    public class Store : ScriptStepGenerator
    {
        public Store() : base("store")
        {
            //AddConfig(new ScriptArgsConfig(0, new ScriptArgsParam[] {
            //        new Params.FsPathScriptArgsParam(),
            //       new Params.EnumScriptArgsParam("type", new string[] { "kernel" }),
            //       new Params.EnumScriptArgsParam("format", new string[] { "gz", "legacy"/*, "fit"*/ }),
            //    }));

            AddConfig(new ScriptArgsConfig(1, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "ramfs" }),
                    new Params.EnumScriptArgsParam("format", new string[] { "cpio", "gz", "legacy" }),
                }));

            AddConfig(new ScriptArgsConfig(2, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "devtree"}),
                    new Params.EnumScriptArgsParam("format", new string[] { "dtb"/*, "fit"*/ }),
                }));

            
            AddConfig(new ScriptArgsConfig(3, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam()
                }));
        }

        public override ScriptStep Get(ScriptArgs Args)
        {
            var A = Args.RawArgs;

            if(Args.ArgConfig == 3)
                return new StoreScriptStep(A[0], "all", "fit");
            else
                return new StoreScriptStep(A[0], A[1], A[2]);
        }

        public class StoreScriptStep : ScriptStep
        {
            string Path;
            string Type;
            string Format;

            public StoreScriptStep(string Path, string Type, string Format) : base("store")
            {
                this.Path = Path;
                this.Type = Type;
                this.Format = Format;
            }

            public override ScriptStepResult Exec(ImageProcessor Processor)
            {
                switch (Type)
                {
                    case "ramfs": return StoreFs(Processor);
                    case "devtree": return StoreDtb(Processor);
                    case "kernel": return StoreKernel(Processor);
                    case "all": return StoreAll(Processor);
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown image type!");
                }
            }

            private ScriptStepResult StoreAll(ImageProcessor Processor)
            {
                switch (Format)
                {
                    case "fit":
                        {
                            var Kernel = Processor.GetKernel();
                            if ((Kernel == null) || !Kernel.Loaded)
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Kernel is not loaded!");

                            var Fs = Processor.GetFs();
                            if ((Fs == null) || (Fs.Loaded == false))
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Filesystem is not loaded!");

                            var Dtb = Processor.GetDevTree();
                            if ((Dtb == null) || !Dtb.Loaded)
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Device tree is not loaded!");

                            ImageFormat.Helper.LogHelper.KernelInfo(Kernel);
                            ImageFormat.Helper.LogHelper.RamfsInfo(Fs, "CPIO");
                            ImageFormat.Helper.LogHelper.DevtreeInfo(Dtb);

                            var Writer = new ImageFormat.Composite.FitWriter(Path);
                            if(Writer.Write(Processor.GetBlob()))
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Images are stored to file {Path} as FIT Image!");
                            else
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Cannot compile FIT Image! No enough information...");
                        }
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown multiimage format!");

                }
            }

            private ScriptStepResult StoreKernel(ImageProcessor Processor)
            {
                switch (Format)
                {
                    case "gz":
                        {
                            var Kernel = Processor.GetKernel();
                            if ((Kernel != null) && Kernel.Loaded)
                            {
                                ImageFormat.Helper.LogHelper.KernelInfo(Kernel);
                                var Exporter = new NyaFs.ImageFormat.Elements.Kernel.Writer.GzWriter(Path);
                                Exporter.WriteKernel(Kernel);
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Kernel is stored to file {Path} as gzipped stream!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Kernel is not loaded!");
                        }
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown devtree format!");
                }
            }

            private ScriptStepResult StoreDtb(ImageProcessor Processor)
            {
                switch (Format)
                {
                    case "dtb":
                        {
                            var Dtb = Processor.GetDevTree();
                            if (Dtb != null)
                            {
                                ImageFormat.Helper.LogHelper.DevtreeInfo(Dtb);

                                var data = new NyaFs.FlattenedDeviceTree.Writer.FDTWriter(Dtb.DevTree);
                                System.IO.File.WriteAllBytes(Path, data.GetBinary());

                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Dtb is stored to file {Path}!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Device tree is not loaded!");
                        }
                    case "dts":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Dts is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown devtree format!");
                }
            }

            private ScriptStepResult StoreFs(ImageProcessor Processor)
            {
                var Fs = Processor.GetFs();
                if((Fs == null) || (Fs.Loaded == false))
                    return new ScriptStepResult(ScriptStepStatus.Error, $"Filesystem is not loaded!");

                switch (Format)
                {
                    case "cpio":
                        {
                            ImageFormat.Helper.LogHelper.RamfsInfo(Fs, "CPIO");
                            var Exporter = new NyaFs.ImageFormat.Elements.Fs.Writer.CpioWriter(Path);
                            Exporter.WriteFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Filesystem is stored to file {Path} as cpio stream!");
                        }
                    case "gz":
                        {
                            ImageFormat.Helper.LogHelper.RamfsInfo(Fs, "CPIO");
                            var Exporter = new NyaFs.ImageFormat.Elements.Fs.Writer.GzCpioWriter(Path);
                            Exporter.WriteFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Filesystem is stored to file {Path} as gzipped cpio stream!");
                        }
                    case "legacy":
                        {
                            ImageFormat.Helper.LogHelper.RamfsInfo(Fs, "CPIO");
                            var Exporter = new NyaFs.ImageFormat.Elements.Fs.Writer.LegacyFsWriter(Path);
                            if (Exporter.CheckFilesystem(Fs))
                            {
                                Exporter.WriteFs(Fs);
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Filesystem is stored to file {Path} as legacy image!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Ok, $"Cannot store as legacy image: no enough info about target system!");
                        }
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown fs format!");
                }
            }
        }
    }
}
