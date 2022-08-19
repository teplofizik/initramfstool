using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Reader
{
    class gzReader : Reader
    {
        byte[] Data = null;

        public gzReader(string Filename)
        {
            Data = System.IO.File.ReadAllBytes(Filename);
        }

        public gzReader(byte[] Data)
        {
            this.Data = Data;
        }

        private string DetectFs(byte[] Raw)
        {
            return "cpio";
        }

        /// <summary>
        /// Читаем в файловую систему из cpio-файла
        /// </summary>
        /// <param name="Dst"></param>
        public override void ReadToFs(Filesystem Dst)
        {
            byte[] Raw = Compressors.Gzip.Decompress(Data);

            var Fs = DetectFs(Raw);
            switch(Fs)
            {
                case "cpio":
                    {
                        var cpr = new CpioReader(Raw);
                        cpr.ReadToFs(Dst);
                    } 
                    break;
                case "ext4":
                    throw new NotImplementedException($"Filesystem type {Fs} not implemented");
            }
        }
    }
}
