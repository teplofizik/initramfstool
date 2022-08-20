using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Types
{
    public class ImageInfo
    {
        public OS  OperatingSystem;
        public CPU Architecture;
        public ImageType Type;

        public string Name;

        public uint DataLoadAddress;
        public uint EntryPointAddress;
    }
}
