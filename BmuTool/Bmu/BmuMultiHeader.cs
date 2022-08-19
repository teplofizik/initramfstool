using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BmuTool.Bmu
{
    class BmuMultiHeader : RawPacket
    {
        // Header:   AB AB AB AB
        // Xxxxxx:   00 00 00 00
        // Size:     24 00 00 00
        // Count:    06 00 00 00
        // ItemSize: AC 00 00 00
        // Xxxxxx:   00 40 00 00
        // Crc:      91 98 DB 70
        // Xxxxxx:   00 00 00 00
        // Xxxxxx:   00 00 00 00

        public bool IsMultiPackage => ReadUInt32(0) == 0xABABABABU;

        public UInt32 Size => ReadUInt32(0x08);
        public UInt32 ItemSize => ReadUInt32(0x10);

        public UInt32 Count => ReadUInt32(0x0C);

        public UInt32 Crc => ReadUInt32(0x18);

        public BmuMultiHeader(byte[] Raw) : base(Raw)
        {
        }
    }
}
