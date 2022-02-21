using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Extension.Packet;
using System.IO.Compression;
using Extension.Array;
using CrcSharp;

namespace RamFsLib.Types
{
    public class InitramFsFile : RawPacket
    {
        public InitramFsFile(byte[] Raw) : base(Raw) { }

        public InitramFsFile(string Filename) : base(File.ReadAllBytes(Filename)) { }

        public InitramFsFile GetModified(byte[] Unpacked)
        {
            var Packed = Compress(Unpacked);
            var Dst = new byte[64 + Packed.Length];

            Array.Copy(ReadArray(0, 64), Dst, 64);
            Array.Copy(Packed, 0, Dst, 64, Packed.Length);

            Dst.WriteUInt32BE(0x0C, Convert.ToUInt32(Packed.Length));
            Dst.WriteUInt32BE(0x18, CalcCrc(Packed));

            var DstFs = new InitramFsFile(Dst);

            return DstFs;
        }

        public long Crc
        {
            get { return ReadUInt32BE(0x18); }
            set { WriteUInt32BE(0x18, Convert.ToUInt32(value)); }
        }
        public bool Correct => CalcCrc(Data) == Crc;

        public long Length
        {
            get { return ReadUInt32BE(0x0C); }
            set { WriteUInt32BE(0x0C, Convert.ToUInt32(value)); }
        }

        public string Name => ReadString(0x20, 0x20);

        public DateTime Timestamp => ConvertFromUnixTimestamp(ReadUInt32BE(0x08));

        // 27 05 19 56 E4 7B B6 14 58 F6 ED 5A 00 A2 0F BA
        // 00 00 00 00 00 00 00 00 A3 6B F6 D2 05 02 03 01
        // 41 6E 67 73 74 72 6F 6D 2D 61 6E 74 6D 69 6E 65
        // 72 5F 6D 2D 65 67 6C 69 62 63 2D 69 70 6B 2D 76

        // Image Name:   Angstrom-antminer_m-eglibc-ipk-v
        // Created:      Mon Feb 21 13:48:18 2022
        // Image Type:   ARM Linux RAMDisk Image(gzip compressed)
        // Data Size:    10658725 Bytes = 10408.91 kB = 10.16 MB
        // Load Address: 00000000
        // Entry Point:  00000000

        public byte[] Data => ReadArray(0x40, Length);

        public byte[] UnpackedData => Decompress(Data);

        static UInt32 CalcCrc(byte[] data)
        {
            var crc32 = new Crc(new CrcParameters(32, 0x04c11db7, 0xffffffff, 0xffffffff, true, true));

            return Convert.ToUInt32(crc32.CalculateAsNumeric(data));
        }
        static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

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
