﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Items
{
    public class SymLink : FilesystemItem
    {
        public string Target;

        public SymLink(string Filename, uint User, uint Group, uint Mode, string Target) : base("symlink", Filename, User, Group, Mode)
        {
            this.Target = Target;
        }

        public override string ToString()
        {
            return $"LINK {Filename} {User}:{Group} {Mode:x03} => {Target}";
        }
    }
}
