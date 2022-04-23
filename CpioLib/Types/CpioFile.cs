using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Extension.Array;
using Extension.Packet;

namespace CpioLib.Types
{
    public class CpioFile : RawPacket
    {
        public static UInt32 MaxNodeId = 0;

        public CpioFile(byte[] Raw) : base(Raw) { }

        private static long CalcPacketSize(string Path, string LocalPath, bool Dir)
        {
            var HeaderSize = 110;
            var PathSize = Path.Length + 1;
            var DataSize = Dir ? 0 : File.ReadAllBytes(LocalPath).Length;
            var HeaderWithPathAlighedSize = Convert.ToInt64(HeaderSize + PathSize).GetAligned(4);
            return (HeaderWithPathAlighedSize + DataSize).GetAligned(4);
        }

        public CpioFile(string Path, string LocalPath, bool Dir) : base(CalcPacketSize(Path, LocalPath, Dir))
        {
            var PathBytes = UTF8Encoding.UTF8.GetBytes(Path); 
            var Data = Dir ? new byte[] { } : File.ReadAllBytes(LocalPath);

            var ModTime = Dir ? new DirectoryInfo(LocalPath).LastWriteTime : new FileInfo(LocalPath).LastWriteTime;

            MaxNodeId++;

            WriteArray(0, UTF8Encoding.UTF8.GetBytes("070701"), 6); // Header
            SetAsciiValue(6, 8, MaxNodeId); // INode []
            SetAsciiValue(14, 8, Dir ? 0x41edU : 0x81a4U); // Mode  [0x41ed dir, 0x81a4 file]

            SetAsciiValue(22, 8, 0); // UserId
            SetAsciiValue(30, 8, 0); // GroupId
            SetAsciiValue(38, 8, 1); // NumLink ???
            SetAsciiValue(46, 8, GetUnixTimestamp(ModTime)); // ModificationTime
            SetAsciiValue(54, 8, Convert.ToUInt32(Data.Length)); // FileSize
            SetAsciiValue(62, 8, 8); // Major
            SetAsciiValue(70, 8, 1); // Minor
            SetAsciiValue(78, 8, 0); // RMajor
            SetAsciiValue(86, 8, 0); // RMinor
            SetAsciiValue(94, 8, Convert.ToUInt32(PathBytes.Length + 1)); // NameSize
            WriteArray(110, PathBytes, PathBytes.Length);
            WriteArray(HeaderWithPathSize, Data, Data.Length);
        }

        public CpioFile UpdateContent(byte[] Data)
        {
            var NewDataSize = HeaderWithPathSize + Data.Length;
            var NewRawSize = NewDataSize.GetAligned(4);

            var NewRaw = new byte[NewRawSize];
            var Header = ReadArray(0, HeaderWithPathSize);
            Array.Copy(Header, NewRaw, HeaderWithPathSize);
            Array.Copy(Data, 0, NewRaw, HeaderWithPathSize, Data.Length);

            var File = new CpioFile(NewRaw);
            File.FileSize = Convert.ToUInt32(Data.Length);

            return File;
        }

        public byte[] Content => ReadArray(HeaderWithPathSize, FileSize);

        public bool IsCorrectMagic => Magic == "070701";

        private UInt32 GetAsciiValue(long HeaderOffset, int Size)
        {
            var Text = ReadString(HeaderOffset, Size);
            return Convert.ToUInt32(Text, 16);
        }

        private void SetAsciiValue(long HeaderOffset, int Size, UInt32 value)
        {
            var Text = $"{value:X08}";
            var Array = UTF8Encoding.UTF8.GetBytes(Text);
            WriteArray(HeaderOffset, Array, Size);
        }

        /// <summary>
        /// The string 070701 for new ASCII, the string 070702 for new ASCII with CRC
        /// </summary>
        public string Magic => ReadString(0, 6);

        // https://developer.adobe.com/experience-manager/reference-materials/6-4/javadoc/org/apache/commons/compress/archivers/cpio/CpioArchiveEntry.html
        public UInt32 INode => GetAsciiValue(6, 8);
        public UInt32 Mode
        {
            get
            {
                return GetAsciiValue(14, 8);
            }
            set
            {
                SetAsciiValue(14, 8, value);
            }
        }

        public UInt32 UserId
        {
            get
            {
                return GetAsciiValue(22, 8);
            }
            set
            {
                SetAsciiValue(22, 8, value);
            }
        }
        public UInt32 GroupId
        {
            get
            {
                return GetAsciiValue(30, 8);
            }
            set
            {
                SetAsciiValue(30, 8, value);
            }
        }

        public UInt32 NumLink => GetAsciiValue(38, 8);
        public UInt32 ModificationTime => GetAsciiValue(46, 8);

        /// <summary>
        /// must be 0 for FIFOs and directories
        /// </summary>
        public UInt32 FileSize
        {
            get { return GetAsciiValue(54, 8); }
            set { SetAsciiValue(54, 8, value); }
        }


        public UInt32 Major => GetAsciiValue(62, 8);
        public UInt32 Minor => GetAsciiValue(70, 8);

        /// <summary>
        /// only valid for chr and blk special files
        /// </summary>
        public UInt32 RMajor => GetAsciiValue(78, 8);

        /// <summary>
        /// only valid for chr and blk special files
        /// </summary>
        public UInt32 RMinor => GetAsciiValue(86, 8);

        /// <summary>
        /// count includes terminating NUL in pathname
        /// </summary>
        public UInt32 NameSize => GetAsciiValue(94, 8);

        /// <summary>
        /// 0 for "new" portable format; for CRC format, the sum of all the bytes in the file
        /// </summary>
        public UInt32 Check => GetAsciiValue(102, 8);

        public CpioModeFlags Flags => (CpioModeFlags)(Mode & 0xFFF);

        public CpioModeFileType FileType => (CpioModeFileType)(Mode & 0x3F000);

        public bool IsTrailer => Path == "TRAILER!!!";

        public long PathPadding => (110L + NameSize).MakeSizeAligned(4);
        public long HeaderWithPathSize => 110 + NameSize + PathPadding;

        public string Path => ReadString(110, Convert.ToInt32(NameSize - 1));

        public long FilePadding => (HeaderWithPathSize + FileSize).MakeSizeAligned(4);

        public long FullFileBlockSize => HeaderWithPathSize + FileSize + FilePadding;

        public override string ToString()
        {
            return $"CPIO: {Path} (sz: {FileSize}, u: {UserId}, g: {GroupId}, m: {Mode})";
        }

        protected UInt32 GetUnixTimestamp(DateTime T) => Convert.ToUInt32(((DateTimeOffset)T).ToUnixTimeSeconds());
    }
}
