using NyaFs.ImageFormat.Dtb;
using NyaFs.ImageFormat.Fs;
using NyaFs.ImageFormat.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat
{
    class BaseImageFormat
    {
        public virtual bool IsProvidedDTB => Dtb != null;
        public virtual bool IsProvidedKernel => Kernel != null;
        public virtual bool IsProvidedFs => Fs != null;

        protected DeviceTree  Dtb = null;
        protected Filesystem  Fs = null;
        protected LinuxKernel Kernel = null;

        public virtual DeviceTree GetDTB(int Index = 0) => Dtb;

        public virtual Filesystem GetFilesystem(int Index = 0) => Fs;

        public virtual LinuxKernel GetKernel(int Index = 0) => Kernel;

        public virtual void Save(string Filename)
        { 
            
        }
    }
}
