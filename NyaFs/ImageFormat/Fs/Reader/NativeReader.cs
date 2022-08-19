using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Reader
{
    public class NativeReader
    {
        private string Dir;
        private uint User;
        private uint Group;
        private uint FileMode;
        private uint DirMode;

        public NativeReader(string Dir, uint User, uint Group, uint FileMode, uint DirMode)
        {
            this.Dir = Dir;
            this.User = User;
            this.Group = Group;
            this.FileMode = FileMode;
            this.DirMode = DirMode;
        }

        /// <summary>
        /// Читаем в файловую систему из папки с диска
        /// </summary>
        /// <param name="Dst"></param>
        public virtual void ReadToFs(Filesystem Dst)
        {
            ProcessDirectory(Dst.Root, Dir);
        }

        private void ProcessDirectory(Items.Dir DirItem, string Path)
        {
            // Process dirs
            var Dirs = System.IO.Directory.GetDirectories(Path);
            foreach (var D in Dirs)
            {
                var RelPath = System.IO.Path.GetRelativePath(Dir, D);
                var CurrentDir = new Items.Dir(RelPath, User, Group, DirMode);
                // Console.WriteLine($"Added dir: {RelPath}");
                DirItem.Items.Add(CurrentDir);

                ProcessDirectory(CurrentDir, D);
            }

            // Process files
            var Files = System.IO.Directory.GetFiles(Path);
            foreach (var F in Files)
            {
                var Data = System.IO.File.ReadAllBytes(F);

                var FilePath = System.IO.Path.GetRelativePath(Dir, F);
                // Console.WriteLine($"Added file: {FilePath} size {Data.Length}");
                var File = new Items.File(FilePath, User, Group, FileMode, Data);

                DirItem.Items.Add(File);
            }
        }
    }
}
