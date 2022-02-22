using System;
using System.Collections.Generic;
using System.Text;

namespace CpioDump
{
    class ArgParser
    {
        Dictionary<string,string> Args = new Dictionary<string, string>();

        public ArgParser(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                var a = args[i];

                if(a.IndexOf('=') > 0)
                {
                    var Parts = a.Split(new char[] { '=' });
                    if (Parts.Length == 2)
                        Args[Parts[0]] = Parts[1];
                }
            }
        }

        public string GetArg(string Name)
        {
            if (Args.ContainsKey(Name))
                return Args[Name];
            else
                return null;
        }
    }
}
