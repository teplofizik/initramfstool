using System;
using System.Collections.Generic;
using System.Text;
using CpioLib.Types;
using CpioLib.IO;

namespace CpioDump.Dumper
{
    class CpioDumper
    {
        CpioArchive Data;

        public CpioDumper(string FileName)
        {
            Data = CpioParser.Load(FileName);
        }

        public void Dump()
        {
            var Files = Data.Files;
            foreach(var F in Files)
            {
                if((F.UserId != 0) || (F.GroupId != 0))
                { 
                    Console.WriteLine($"{F}");
                }
            }
        }
    }
}
