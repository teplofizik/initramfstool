using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Writer
{
    public class LegacyFsWriter : Writer
    {
        string Filename;
        Types.ImageInfo Info;

        public LegacyFsWriter(string Filename, Types.ImageInfo Info)
        {
            this.Info = Info;
            this.Filename = Filename;
        }

        public override void WriteFs(Filesystem Fs)
        {
            var CpWriter = new CpioWriter();
            CpWriter.WriteFs(Fs);

            var PackedData = Compressors.Gzip.Compress(CpWriter.RawStream);

            var Image = new Types.LegacyImage(Info, PackedData);

            System.IO.File.WriteAllBytes(Filename, Image.getPacket());
        }
    }
}
