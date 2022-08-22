using NyaFs.ImageFormat.Dtb;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor.Scripting.Commands
{
    public class Load : ScriptStepGenerator
    {
        public Load() : base("load")
        {
            AddConfig(new ScriptArgsConfig(0, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new ImageTypeScriptArgsParam(),
                    new ImageFormatScriptArgsParam()
                }));

            AddConfig(new ScriptArgsConfig(1, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new ImageTypeDtScriptArgsParam()
                }));
        }

        public override ScriptStep Get(ScriptArgs Args)
        {
            var A = Args.RawArgs;

            if(Args.ArgConfig == 0)
                return new LoadScriptStep(A[0], A[1], A[2]);
            else
                return new LoadScriptStep(A[0], A[1], "dtb");
        }

        public class LoadScriptStep : ScriptStep
        {
            string Path;
            string Type;
            string Format;

            public LoadScriptStep(string Path, string Type, string Format) : base("load")
            {
                this.Path = Path;
                this.Type = Type;
                this.Format = Format;
            }

            public override ScriptStepResult Exec(ImageProcessor Processor)
            {
                // Проверим наличие локального файла, содержимое которого надо загрузить в ФС
                if (!System.IO.File.Exists(Path))
                    return new ScriptStepResult(ScriptStepStatus.Error, $"{Path} not found!");

                switch(Type)
                {
                    case "ramfs": return ReadFs(Processor);
                    case "devtree": return ReadDtb(Processor);
                    case "kernel":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Kernel loading is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown image type!");
                }
            }

            private ScriptStepResult ReadDtb(ImageProcessor Processor)
            {
                var Dtb = new FlattenedDeviceTree.Reader.FDTReader(Path).Read();
                Processor.SetDeviceTree(new DeviceTree(Dtb));

                return new ScriptStepResult(ScriptStepStatus.Ok, $"Dtb is loaded!");
            }

            private ScriptStepResult ReadFs(ImageProcessor Processor)
            {
                var Fs = new NyaFs.ImageFormat.Fs.Filesystem();
                switch (Format)
                {
                    case "cpio":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.CpioReader(Path);
                            Importer.ReadToFs(Fs);
                            Processor.SetFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Cpio is loaded as filesystem!");
                        }
                    case "gz":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.gzReader(Path);
                            Importer.ReadToFs(Fs);
                            Processor.SetFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"gz file is loaded as filesystem!");
                        }
                    case "legacy":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.LegacyFsReader(Path);
                            Importer.ReadToFs(Fs);
                            Processor.SetFs(Fs);
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"legacy file is loaded as filesystem!");
                        }
                    case "fit":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Fit format is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown fs format!");
                }
            }
        }

        class ImageTypeDtScriptArgsParam : ScriptArgsParam
        {
            public ImageTypeDtScriptArgsParam() : base("imagetype") { }

            public override bool CheckParam(string Arg) => (Arg == "devtree");
        }

        class ImageTypeScriptArgsParam : ScriptArgsParam
        {
            public ImageTypeScriptArgsParam() : base("imagetype") { }

            public override bool CheckParam(string Arg) => (Arg == "kernel") || (Arg == "ramfs");
        }

        class ImageFormatScriptArgsParam : ScriptArgsParam
        {
            public ImageFormatScriptArgsParam() : base("imageformat") { }

            public override bool CheckParam(string Arg) => (Arg == "cpio") || (Arg == "gz") || (Arg == "legacy") || (Arg == "fit");
        }
    }
}
