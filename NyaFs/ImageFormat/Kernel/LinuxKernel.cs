using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Kernel
{
    public class LinuxKernel
    {
        /// <summary>
        /// Image information, arch or supported os
        /// </summary>
        public Types.ImageInfo Info = new Types.ImageInfo();

        /// <summary>
        /// Несжатый образ ядра
        /// </summary>
        public byte[] Image = null;
    }
}
