using CpioLib.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Extension.Array;

namespace CpioLib.IO
{
    public static class CpioPacker
    {
        public static void Save(CpioArchive Archive, string FileName)
        {
            var Res = new List<byte>();

            foreach (var F in Archive.Files)
                Res.AddRange(F.getPacket());

            var Padding = Convert.ToInt64(Res.Count).MakeSizeAligned(0x1000);
            for (long i = 0; i < Padding; i++) Res.Add(0);

            File.WriteAllBytes(FileName, Res.ToArray());
        }
    }
}
