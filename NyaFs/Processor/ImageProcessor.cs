using NyaFs.ImageFormat.Elements.Dtb;
using NyaFs.ImageFormat.Elements.Fs;
using NyaFs.ImageFormat.Elements.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor
{
    public class ImageProcessor
    {
        Filesystem Fs;
        LinuxKernel Kernel;
        DeviceTree Dtb;

        public void SetFs(Filesystem Fs)
        {
            this.Fs = Fs;
        }

        public void SetKernel(LinuxKernel Kernel)
        {
            this.Kernel = Kernel;
        }

        public void SetDeviceTree(DeviceTree Dtb)
        {
            this.Dtb = Dtb;
        }

        public LinuxKernel GetKernel()
        {
            return Kernel;
        }

        public Filesystem GetFs()
        {
            return Fs;
        }

        public DeviceTree GetDevTree()
        {
            return Dtb;
        }

        public void Process(Scripting.Script Script)
        {
            foreach(var S in Script.Steps)
            {
                var Res = S.Exec(this);

                WriteLogLine(S, Res);
            }
        }

        private void WriteLogLine(Scripting.ScriptStep Step, Scripting.ScriptStepResult Res)
        {
            switch(Res.Status)
            {
                case ScriptStepStatus.Error: Log.Error(0, $"{Step.Name} [{Step.ScriptName}:{Step.ScriptLine}]: {Res.Text}"); break;
                case ScriptStepStatus.Ok: Log.Ok(0, $"{Step.Name}: {Res.Text}"); break;
                case ScriptStepStatus.Warning: Log.Warning(0, $"{Step.Name} [{Step.ScriptName}:{Step.ScriptLine}]: {Res.Text}"); break;
            }
        }
    }
}
