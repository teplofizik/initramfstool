using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Items
{
    public class File : FilesystemItem
    {
        public byte[] Content;

        public File(string Filename, uint User, uint Group, uint Mode, byte[] Content) : base("file", Filename, User, Group, Mode)
        {
            this.Content = Content;
        }

        public override string ToString()
        {
            return $"FILE {Filename} {User}:{Group} {Mode:x03} {Content.Length} bytes";
        }
    }
}
