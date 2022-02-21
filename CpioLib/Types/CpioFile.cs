using System;
using System.Collections.Generic;
using System.Text;

using Extension.Array;
using Extension.Packet;

namespace CpioLib.Types
{
    public class CpioFile : RawPacket
    {
        public CpioFile(byte[] Raw) : base(Raw) { }

        public CpioFile UpdateContent(byte[] Data)
        {
            var NewDataSize = HeaderWithPathSize + Data.Length;
            var NewRawSize = NewDataSize.MakeSizeAligned(4);

            var NewRaw = new byte[NewRawSize];
            var Header = ReadArray(0, HeaderWithPathSize);
            Array.Copy(Header, NewRaw, HeaderWithPathSize);
            Array.Copy(Data, 0, NewRaw, HeaderWithPathSize, Data.Length);

            return new CpioFile(NewRaw);
        }

        public bool IsCorrectMagic => Magic == "070701";

        private UInt32 GetAsciiValue(long HeaderOffset, int Size)
        {
            var Text = ReadString(HeaderOffset, Size);
            return Convert.ToUInt32(Text, 16);
        }

        /// <summary>
        /// The string 070701 for new ASCII, the string 070702 for new ASCII with CRC
        /// </summary>
        public string Magic => ReadString(0, 6);

        // https://developer.adobe.com/experience-manager/reference-materials/6-4/javadoc/org/apache/commons/compress/archivers/cpio/CpioArchiveEntry.html
        public UInt32 INode => GetAsciiValue(6, 8);
        public UInt32 Mode => GetAsciiValue(14, 8);
        public UInt32 UserId => GetAsciiValue(22, 8);
        public UInt32 GroupId => GetAsciiValue(30, 8);
        public UInt32 NumLink => GetAsciiValue(38, 8);
        public UInt32 ModificationTime => GetAsciiValue(46, 8);

        /// <summary>
        /// must be 0 for FIFOs and directories
        /// </summary>
        public UInt32 FileSize => GetAsciiValue(54, 8);


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

        public CpioModeFlags Flags => (CpioModeFlags)(Mode & 07777u);

        public CpioModeFileType FileType => (CpioModeFileType)(Mode & 0770000u);

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
    }
}
