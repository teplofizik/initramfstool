using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Writer
{
    public class Writer
    {
        public virtual void WriteFs(Filesystem Fs)
        {

        }

        public virtual bool HasRawStreamData => false;

        public virtual byte[] RawStream => null;
    }
}
