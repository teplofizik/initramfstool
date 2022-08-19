using Extension.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BmuTool.Bmu
{
    class BmuSingleHeader : RawPacket
    {
        //
        // 26 01 75 DA 6E 7B FE B7 B5 08 01 02
        // Xxxx: 00 00 00 00
        // Xxxx: 00 00 00 00
        // Size: 00 00 01 C3

        // 0x0000: PUBLIC KEY
        // 0x0400: ???
        // 0x0800: ANDROID KERNEL

        public UInt32 Size => ReadUInt32(0x14);

        public BmuSingleHeader(byte[] Raw) : base(Raw)
        {

        }

        public byte[] Kernel => ReadArray(0x800, getLength() - 0x800);

        public bool IsAndroid => String.Equals(ReadString(0x800, 8), "ANDROID!");

        public byte[] Decompressed => Decompress(Kernel);

        static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
