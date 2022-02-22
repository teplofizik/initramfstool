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
            File.WriteAllBytes(FileName, GetRawData(Archive));
        }

        public static byte[] GetRawData(CpioArchive Archive)
        {
            var Res = new List<byte>();

            foreach (var F in Archive.Files)
                Res.AddRange(F.getPacket());

            if (Archive.Trailer != null)
            {
                Res.AddRange(Archive.Trailer.getPacket());
            }

            var Padding = Convert.ToInt64(Res.Count).MakeSizeAligned(0x100);
            for (long i = 0; i < Padding; i++) Res.Add(0);
            return Res.ToArray();
        }
    }
}
