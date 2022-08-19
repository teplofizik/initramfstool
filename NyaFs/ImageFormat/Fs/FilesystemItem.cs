using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs
{
    public class FilesystemItem
    {
        public FilesystemItem(string Type, string Filename, uint User, uint Group, uint Mode)
        {
            this.ItemType = Type;

            this.User = User;
            this.Group = Group;
            this.Mode = Mode;

            this.Filename = Filename;
        }

        public readonly string ItemType;

        public uint User;
        public uint Group;

        public uint Mode; // Access rights 12 bit 4 4 4

        public string Filename;

        public uint Major = 8;
        public uint Minor = 1;
        public uint RMajor = 0;
        public uint RMinor = 0;
    }
}
