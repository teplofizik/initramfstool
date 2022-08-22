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
                    new Params.EnumScriptArgsParam("type", new string[] { "kernel" }),
                    new Params.EnumScriptArgsParam("format", new string[] { "gz", "legacy"/*, "fit"*/ }),
                }));

            AddConfig(new ScriptArgsConfig(1, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "ramfs" }),
                    new Params.EnumScriptArgsParam("format", new string[] { "cpio", "gz", "legacy"/*, "fit"*/ }),
                }));

            AddConfig(new ScriptArgsConfig(2, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "devtree"}),
                    new Params.EnumScriptArgsParam("format", new string[] { "dtb"/*, "fit"*/ }),
                }));

            /*
            AddConfig(new ScriptArgsConfig(3, new ScriptArgsParam[] {
                    new Params.FsPathScriptArgsParam(),
                    new Params.EnumScriptArgsParam("type", new string[] { "all" }),
                    new Params.EnumScriptArgsParam("format", new string[] { "fit" }),
                }));*/
        }

        public override ScriptStep Get(ScriptArgs Args)
        {
            var A = Args.RawArgs;

            return new LoadScriptStep(A[0], A[1], A[2]);
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
                switch (Format)
                {
                    case "dtb":
                        {
                            var Dtb = new FlattenedDeviceTree.Reader.FDTReader(Path).Read();
                            Processor.SetDeviceTree(new DeviceTree(Dtb));
                            return new ScriptStepResult(ScriptStepStatus.Ok, $"Dtb is loaded!");
                        }
                    case "fit":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Fit format is not supported now!");
                    case "dts":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Dts is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown devtree format!");
                }
            }

            private ScriptStepResult ReadFs(ImageProcessor Processor)
            {
                var OldLoaded = Processor.GetFs()?.Loaded ?? false;
                var Fs = new NyaFs.ImageFormat.Fs.Filesystem();
                switch (Format)
                {
                    case "cpio":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.CpioReader(Path);
                            Importer.ReadToFs(Fs);
                            if (Fs.Loaded)
                            {
                                Processor.SetFs(Fs);
                                if(OldLoaded)
                                    return new ScriptStepResult(ScriptStepStatus.Warning, $"Cpio is loaded as filesystem! Old filesystem is replaced by this.");
                                else
                                    return new ScriptStepResult(ScriptStepStatus.Ok, $"Cpio is loaded as filesystem!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"Cpio is not loaded!");
                        }
                    case "gz":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.gzReader(Path);
                            Importer.ReadToFs(Fs);
                            if (Fs.Loaded)
                            {
                                Processor.SetFs(Fs);
                                if (OldLoaded)
                                    return new ScriptStepResult(ScriptStepStatus.Warning, $"Cpio is loaded as filesystem! Old filesystem is replaced by this.");
                                else
                                    return new ScriptStepResult(ScriptStepStatus.Ok, $"gz file is loaded as filesystem!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"gz file is not loaded!");
                        }
                    case "legacy":
                        {
                            var Importer = new NyaFs.ImageFormat.Fs.Reader.LegacyFsReader(Path);
                            Importer.ReadToFs(Fs);
                            if (Fs.Loaded)
                            {
                                Processor.SetFs(Fs);
                                if (OldLoaded)
                                    return new ScriptStepResult(ScriptStepStatus.Warning, $"Cpio is loaded as filesystem! Old filesystem is replaced by this.");
                                else
                                    return new ScriptStepResult(ScriptStepStatus.Ok, $"legacy file is loaded as filesystem!");
                            }
                            else
                                return new ScriptStepResult(ScriptStepStatus.Error, $"legacy file is not loaded!");
                        }
                    case "fit":
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Fit format is not supported now!");
                    default:
                        return new ScriptStepResult(ScriptStepStatus.Error, $"Unknown fs format!");
                }
            }
        }
    }
}
