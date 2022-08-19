using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BmuTool.Bmu
{
    class AndroidKernelHeader : RawPacket
    {
        public string Magic => ReadString(0x00, 8); // ANDROID!

        /// <summary>
        /// size in bytes: 00 20 5C 00
        /// </summary>
        public UInt32 kernel_size => ReadUInt32(0x08);
        /// <summary>
        /// physical load addr: 00 00 08 01
        /// </summary>
        public UInt32 kernel_addr => ReadUInt32(0x0C);

        /// <summary>
        /// size in bytes: 00 20 66 00
        /// </summary>
        public UInt32 ramdisk_size => ReadUInt32(0x10);
        /// <summary>
        /// physical load addr: 00 00 00 01
        /// </summary>
        public UInt32 ramdisk_addr => ReadUInt32(0x14);

        /// <summary>
        /// size in bytes: 00 78 00 00
        /// </summary>
        public UInt32 second_size => ReadUInt32(0x18);
        /// <summary>
        /// physical load addr: 00 00 F0 00
        /// </summary>
        public UInt32 second_addr => ReadUInt32(0x1C);

        /// <summary>
        /// physical addr for kernel tags: 00 01 00 00
        /// </summary>
        public UInt32 tags_addr => ReadUInt32(0x20);
        /// <summary>
        /// flash page size we assume: 00 08 00 00
        /// </summary>
        public UInt32 page_size => ReadUInt32(0x24);
        /// <summary>
        /// future expansion: should be 0: 00 00 00 00 00 00 00 00
        /// </summary>
        public UInt64 unused => ReadUInt32(0x28);

        /// <summary>
        /// asciiz product name
        /// </summary>
        public string name => ReadString(0x30, 0x10);

        /// <summary>
        /// cmdline
        /// </summary>
        public string cmdline => ReadString(0x40, 512);

        /*  put a hash of the contents in the header so boot images can be
            differentiated based on their first 2k.
        SHA_init(&ctx);
        SHA_update(&ctx, kernel_data, hdr.kernel_size);
        SHA_update(&ctx, &hdr.kernel_size, sizeof(hdr.kernel_size));
        SHA_update(&ctx, ramdisk_data, hdr.ramdisk_size);
        SHA_update(&ctx, &hdr.ramdisk_size, sizeof(hdr.ramdisk_size));
        SHA_update(&ctx, second_data, hdr.second_size);
        SHA_update(&ctx, &hdr.second_size, sizeof(hdr.second_size));
        sha = SHA_final(&ctx); */
        public UInt32 Id1 => ReadUInt32(0x240); // 3E 97 53 98
        public UInt32 Id2 => ReadUInt32(0x244); // 71 D5 11 9D
        public UInt32 Id3 => ReadUInt32(0x248); // 21 E4 60 FD
        public UInt32 Id4 => ReadUInt32(0x24C); // AB B1 90 A8
        public UInt32 Id5 => ReadUInt32(0x250); // EF 48 36 C5
        public UInt32 Id6 => ReadUInt32(0x254); // 00 00 00 00
        public UInt32 Id7 => ReadUInt32(0x258); // 00 00 00 00
        public UInt32 Id8 => ReadUInt32(0x25C); // 00 00 00 00
        /*
        ** +-----------------+ 
        ** | boot header     | 1 page
        ** +-----------------+
        ** | kernel          | n pages  
        ** +-----------------+
        ** | ramdisk         | m pages  
        ** +-----------------+
        ** | second stage    | o pages
        ** +-----------------+
        **
        ** n = (kernel_size + page_size - 1) / page_size
        ** m = (ramdisk_size + page_size - 1) / page_size
        ** o = (second_size + page_size - 1) / page_size
        **
        ** 0. all entities are page_size aligned in flash
        ** 1. kernel and ramdisk are required (size != 0)
        ** 2. second is optional (second_size == 0 -> no second)
        ** 3. load each element (kernel, ramdisk, second) at
        **    the specified physical address (kernel_addr, etc)
        ** 4. prepare tags at tag_addr.  kernel_args[] is
        **    appended to the kernel commandline in the tags.
        ** 5. r0 = 0, r1 = MACHINE_TYPE, r2 = tags_addr
        ** 6. if second_size != 0: jump to second_addr
        **    else: jump to kernel_addr
        */
        public AndroidKernelHeader(byte[] Raw) : base(Raw)
        {

        }
    }
}
