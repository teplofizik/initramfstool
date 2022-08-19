using Extension.Array;
using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BmuTool.Bmu
{
    class BmuParser
    {
        public void Parse(byte[] Raw, string Dir)
        {
            var Sig = Raw.ReadUInt32(0);

            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            if (Sig == 0xABABABABU)
            {
                // Multi package
                var Header = new BmuMultiHeader(Raw.ReadArray(0x00, 0x24));
                Log($"Multi package");
                Log($"Image count: {Header.Count}");
                Log($"  Item size: {Header.ItemSize:X2}");
                Log($"        CRC: {Header.Count:X8}");

                UInt32 Offset = Header.Size;
                for(int i = 0; i < Header.Count; i++)
                {
                    var Item = new BmuPackageItemHeader(Raw.ReadArray(Offset + Header.ItemSize * i, Header.ItemSize));

                    Log($"  =========================================");
                    Log($"  Package {i}");
                    Log($"  Flags   : {Item.Flags:X8}");
                    Log($"  Filename: {Item.Filename}");
                    Log($"    System: {Item.System}");
                    Log($"      Name: {Item.Name}");
                    Log($"    Offset: {Item.Offset:X8}");
                    Log($"    Length: {Item.Size:X8}");

                    var RawFile = Raw.ReadArray(Item.Offset, Item.Size);

                    var SystemDir = Path.Combine(Dir, Item.System);
                    if (!Directory.Exists(SystemDir))
                        Directory.CreateDirectory(SystemDir);

                    var SingleBmuPath = Path.Combine(SystemDir, Item.Filename);
                    if (!File.Exists(SingleBmuPath))
                        File.WriteAllBytes(SingleBmuPath, RawFile);

                    var Bmu = new BmuSingleHeader(RawFile);
                    if(Bmu.IsAndroid)
                    {
                        Log($"   Android: found");

                        var KernelImage = Bmu.Kernel;
                        var Decompressed = Bmu.Decompressed;

                        var KernelPath = Path.Combine(SystemDir, "uImage");
                        File.WriteAllBytes(KernelPath, KernelImage);

                        var Android = new AndroidKernelHeader(KernelImage);

                        Log($"       KernelSize: {Android.kernel_size:X8}");
                        Log($"       KernelAddr: {Android.kernel_addr:X8}");
                        Log($"      RamDiskSize: {Android.ramdisk_size:X8}");
                        Log($"      RamDiskAddr: {Android.ramdisk_addr:X8}");
                        Log($"       SecondSize: {Android.second_size:X8}");
                        Log($"       SecondAddr: {Android.second_addr:X8}");

                        Log($"         PageSize: {Android.page_size:X8}");

                        Log($"             Name: {Android.name}");
                        Log($"          Cmdline: {Android.cmdline}");

                        // SHA Digest
                        Log($"            Id[0]: {Android.Id1:X8}");
                        Log($"            Id[1]: {Android.Id2:X8}");
                        Log($"            Id[2]: {Android.Id3:X8}");
                        Log($"            Id[3]: {Android.Id4:X8}");
                        Log($"            Id[4]: {Android.Id5:X8}");
                        Log($"            Id[5]: {Android.Id6:X8}");
                        Log($"            Id[6]: {Android.Id7:X8}");
                        Log($"            Id[7]: {Android.Id8:X8}");

                        var EncHeader = new EncryptBootImgInfo(KernelImage.ReadArray(0x400, 1024));
                        if(EncHeader.IsAvailable)
                        {
                            Log($"        Encrypted: true");
                            Log($"          StructVers: {EncHeader.Version}");
                            Log($"            TotalLen: {EncHeader.TotalLenAfterEncrypted:X4}");
                        }


                    }
                }
            }
            else
            {
                // Package
            }
        }

        public void Log(string Text)
        {
            Debug.WriteLine(Text);
        }

    }
}
