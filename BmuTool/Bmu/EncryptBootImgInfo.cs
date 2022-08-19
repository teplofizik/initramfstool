using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BmuTool.Bmu
{
    class EncryptBootImgInfo : RawPacket
    {
        // https://github.com/codesnake/uboot-amlogic/blob/master/common/cmd_imgread.c#L28

        public bool IsAvailable => String.Equals("AMLSECU!", ReadString(0x00, 0x08));

        public UInt32 Version => ReadUInt32(0x08);
        public UInt32 TotalLenAfterEncrypted => ReadUInt32(0x0C);

        public EncryptBootImgInfo(byte[] Raw) : base(Raw)
        {

        }
    }
}
