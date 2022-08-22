using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor.Scripting
{
    public class ScriptStep
    {
        public string ScriptName;
        public int ScriptLine;

        public readonly string Name;

        public ScriptStep(string Name)
        {
            this.Name = Name;
        }

        public void SetScriptInfo(string Name, int Line)
        {
            ScriptName = Name;
            ScriptLine = Line;
        }

        public virtual ScriptStepResult Exec(ImageProcessor Processor)
        {
            return new ScriptStepResult(ScriptStepStatus.Error, "Override default Exec function");
        }
    }
}
