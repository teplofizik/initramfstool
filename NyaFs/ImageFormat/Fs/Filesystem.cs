using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs
{
    // cpio
    // gzipped cpio
    // ext4
    // gzipped ext4
    // legacy gzipped cpio
    // legacy gzipped ext4
    // fit => gzipped cpio
    // fit => gzipped ext4
    public class Filesystem
    {
        public Items.Dir Root = new Items.Dir(".", 0, 0, 0x755);
        public Types.ImageInfo Info = new Types.ImageInfo();

        public void Dump()
        {
            DumpDir(Root);
        }

        private void DumpDir(Items.Dir Dir)
        {
            Console.WriteLine(Dir.ToString());
            foreach(var I in Dir.Items)
            {
                if (I.ItemType == Types.FilesystemItemType.Dir)
                    DumpDir(I as Items.Dir);
                else
                    Console.WriteLine(I.ToString());
            }
        }

        public string GetParentDir(string Path)
        {
            int Pos = Path.LastIndexOf('/');
            if (Pos >= 0)
                return Path.Substring(0, Pos);
            else
                return ".";
        }

        public Items.Dir GetDirectory(string Path)
        {
            var Element = GetElement(Path);

            return Element as Items.Dir;
        }

        public FilesystemItem GetElement(string Path)
        {
            if (Path == ".") return Root;
            if(Path.Length == 0)
                throw new ArgumentException($"{Path} is empty");

            if (Path[0] == '/') Path = Path.Substring(1);

            var Parts = Path.Split("/");

            Items.Dir Base = Root; 
            string Rel = null;
            foreach(var P in Parts)
            {
                Rel = (Rel == null) ? P : Rel + "/" + P;

                bool Found = false;
                foreach(var I in Base.Items)
                {
                    if(I.Filename == Rel)
                    {
                        if (Rel == Path)
                            return I;

                        if (I.ItemType == Types.FilesystemItemType.Dir)
                        {
                            Base = I as Items.Dir;
                            Found = true;
                            break;
                        }
                        else
                            throw new ArgumentException($"{Rel} is not dir, cannot process to {Path} node");
                    }
                }
                if(!Found)
                    throw new ArgumentException($"{Rel} is not found in filesystem");
            }

            throw new ArgumentException($"{Path} is not found in filesystem");
        }

      //  public FilesystemItem Get(string Path)
      //  {

      //  }
    }
}
