using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CpioLib.Types;
using Extension.Array;

namespace CpioLib.IO
{
    public static class CpioParser
    {
        public static CpioArchive Load(string Filename)
        {
            var Res = new CpioArchive();
            var Data = File.ReadAllBytes(Filename);

            long Offset = 0;
            while(Offset < Data.Length)
            {
                var FI = new CpioFileInfo(Data, Offset);

                if (FI.IsCorrectMagic)
                {
                    var Raw = Data.ReadArray(Offset, FI.FullFileBlockSize);
                    var F = new CpioFile(Raw);

                    Res.Files.Add(F);

                    Offset += FI.FullFileBlockSize;
                    if (FI.IsTrailer)
                        break;
                }
                else
                    break;
            }

            return Res;
        }

    }
}
