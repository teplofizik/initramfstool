using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Types
{
    public class ImageInfo
    {
        public OS  OperatingSystem = OS.IH_OS_INVALID;
        public CPU Architecture = CPU.IH_ARCH_INVALID;
        public ImageType Type = ImageType.IH_TYPE_INVALID;

        public string Name = "";

        public uint DataLoadAddress = 0;
        public uint EntryPointAddress = 0;
    }
}
