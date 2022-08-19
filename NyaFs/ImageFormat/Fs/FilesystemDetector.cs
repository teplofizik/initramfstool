using Extension.Array;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs
{
    static class FilesystemDetector
    {
        public static Types.FsType DetectFs(byte[] Raw)
        {
            if (Raw.ReadUInt32BE(0) == 0x30373037u)
                return Types.FsType.Cpio;

            return Types.FsType.Unknown;
        }
    }
}
