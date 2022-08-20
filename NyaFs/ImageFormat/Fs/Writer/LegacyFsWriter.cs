using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Writer
{
    public class LegacyFsWriter : Writer
    {
        string Filename;

        public LegacyFsWriter(string Filename)
        {
            this.Filename = Filename;
        }

        public override void WriteFs(Filesystem Fs)
        {
            var CpWriter = new CpioWriter();
            CpWriter.WriteFs(Fs);

            var PackedData = Compressors.Gzip.Compress(CpWriter.RawStream);

            var Image = new Types.LegacyImage(Fs.Info, PackedData);

            System.IO.File.WriteAllBytes(Filename, Image.getPacket());
        }
    }
}
