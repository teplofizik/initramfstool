using System;
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

            /*
            AddConfig(new ScriptArgsConfig(1, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "all" }),
                    new Params.EnumScriptArgsParam("format", new string[] { "fit" }),
                }));*/
        }

        public override ScriptStep Get(ScriptArgs Args)
        {
            var A = Args.RawArgs;

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
                    case "kernel":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Kernel loading is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown image type!");
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
                            var Exporter = new NyaFs.ImageFormat.Fs.Writer.CpioWriter(Path);
                            Exporter.WriteFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Filesystem is stored to file {Path} as cpio stream!");
                        }
                    case "gz":
                        {
                            var Exporter = new NyaFs.ImageFormat.Fs.Writer.GzCpioWriter(Path);
                            Exporter.WriteFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Filesystem is stored to file {Path} as gzipped cpio stream!");
                        }
                    case "legacy":
                        {
                            var Exporter = new NyaFs.ImageFormat.Fs.Writer.LegacyFsWriter(Path);
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
