using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BmuTool.Bmu
{
    class BmuPackageItemHeader : RawPacket
    {
        // Antminer_S19j_Pro_merge_release_20211021062136_6IN1_download12_01.bmu
        // 0x24: 0A 00 11 11 update.bmu [0x60] zynq7007_BHB42601 [0x20] Antminer S19j Pro [0x20] 00 40 00 00 85 9C 11 01
        // 0xD0: 0A 00 0F 11 update.bmu [0x60] BBCtrl_BHB42601 [0x20] Antminer S19j Pro [0x20] 85 DC 11 01 AD 23 A4 00
        // 
        // Header [4]
        // Filename [0x60]
        // System [0x20]
        // Name [0x20]
        // Offset [4]
        // Length [4]

        public UInt32 Flags => ReadUInt32(0x00);
        public string Filename => ReadString(0x04, 0x60);
        public string System => ReadString(0x64, 0x20);
        public string Name => ReadString(0x84, 0x20);
        public UInt32 Offset => ReadUInt32(0xA4);
        public UInt32 Size => ReadUInt32(0xA8);

        public BmuPackageItemHeader(byte[] Raw) : base(Raw)
        {

        }
    }
}
