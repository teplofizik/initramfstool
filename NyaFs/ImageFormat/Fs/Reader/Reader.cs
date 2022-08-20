using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Reader
{
    public class Reader
    {
        /// <summary>
        /// Читаем в файловую систему из внешнего источника
        /// </summary>
        /// <param name="Dst"></param>
        public virtual void ReadToFs(Filesystem Dst)
        {

        }

        public virtual Types.ImageInfo GetImageInfo() => null;

        protected void AddFile(Filesystem Dst, Items.File File)
        {

        }
    }
}
