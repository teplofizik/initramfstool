using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Elements.Kernel.Reader
{
    class GzReader : Reader
    {
        byte[] Data = null;

        public GzReader(string Filename)
        {
            Data = System.IO.File.ReadAllBytes(Filename);
        }

        public GzReader(byte[] Data)
        {
            this.Data = Data;
        }

        public override void ReadToKernel(LinuxKernel Dst)
        {
            byte[] Raw = Compressors.Gzip.Decompress(Data);

            Dst.Image = Raw;
            Helper.LogHelper.KernelInfo(Dst);
        }
    }
}
