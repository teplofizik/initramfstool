using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.Processor.Scripting
{
    public class ScriptParser
    {
        public Script Script = new Script();
        private ScriptBase Base;

        public ScriptParser(ScriptBase Base, string Filename) : this(Base, Filename, System.IO.File.ReadAllLines(Filename))
        {

        }

        public ScriptParser(ScriptBase Base, string Name, string[] Lines)
        {
            this.Base = Base;

            Parse(Name, Lines);
        }

        /// <summary>
        /// Extract command args (all elements after first)
        /// </summary>
        /// <param name="Parts">Splitted command line</param>
        /// <returns></returns>
        private string[] ExtractArgs(string[] Parts)
        {
            string[] Args = new string[Parts.Length - 1];

            for (int i = 0; i < Parts.Length - 1; i++)
                Args[i] = Parts[i + 1];

            return Args;
        }

        /// <summary>
        /// Process a set of lines
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Lines"></param>
        private void Parse(string Filename, string[] Lines)
        {
            var Sep = new char[] { ' ' };
            for(int i = 0; i < Lines.Length; i++)
            {
                var TCmd = Lines[i].Trim();
                if (TCmd.StartsWith('#')) continue;
                if (TCmd.Length == 0) continue;

                var Parts = TCmd.Split(Sep);

                var Command = Parts[0];
                var Args = ExtractArgs(Parts);

                ParseLine(Filename, i + 1, Command, Args);
            }
        }

        private void ParseLine(string Filename, int Line, string Cmd, string[] Args)
        {
            var Gen = Base.GetGenerator(Cmd);

            if (Gen == null)
            {
                Script.HasErrors = true;
                Log.Error(0, $"Error at {Filename}:{Line}. Command {Cmd} is not supported.");
            }
            else
            {
                // Check and select args configuration
                var SArgs = Gen.GetArgs(Args);
                if (SArgs != null)
                {
                    var Step = Gen.Get(SArgs);
                    Step.SetScriptInfo(Filename, Line);

                    Script.Steps.Add(Step);
                }
                else
                {
                    Script.HasErrors = true;
                    Log.Error(0, $"Error at {Filename}:{Line}. Invalid arguments for command '{Cmd}'.");
                }
            }
        }
    }
}
