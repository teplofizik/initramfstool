using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CpioLib.Types
{
    public class CpioArchive
    {
        public CpioFile Trailer = null;
        public List<CpioFile> Files = new List<CpioFile>();

        public CpioArchive()
        {

        }

        public bool Exists(string Filename)
        {
            foreach (var F in Files)
            {
                if (F.Path == Filename)
                    return true;
            }
            return false;
        }

        public CpioFile GetFile(string Filename)
        {
            foreach (var F in Files)
            {
                if (F.Path == Filename)
                    return F;
            }
            return null;
        }

        public CpioFile[] GetFiles(string Path)
        {
            var Res = new List<CpioFile>();
            foreach (var F in Files)
            {
                if (F.Path.StartsWith(Path))
                    Res.Add(F);
            }
            return Res.ToArray();
        }

        protected string[] ProcessFiles(string Path, Func<CpioFile,bool,bool> Action)
        {
            var Res = new List<string>();
            if (Path.Last() == '/')
            {
                var Files = GetFiles(Path);
                foreach (var F in Files)
                {
                    var ARes = Action?.Invoke(F, true);
                    if (ARes.HasValue && ARes.Value)
                        Res.Add(F.Path);
                }

            }
            else
            {
                var F = GetFile(Path);
                if (F != null)
                {
                    var ARes = Action?.Invoke(F, false);
                    if (ARes.HasValue && ARes.Value)
                        Res.Add(F.Path);
                }
            }
            return Res.ToArray();
        }

        public void Delete(string Filename)
        {
            Files.RemoveAll(F => (F.Path == Filename));
        }

        public string[] ChMod(string Filename, UInt32 Mode)
        {
            return ProcessFiles(Filename, (F,List) =>
                {
                    if ((F.FileType == CpioModeFileType.C_ISREG) || !List)
                    {
                        // rwxr-x---
                        var OldMode = F.Mode & ~0x1FFU;

                        OldMode |= (Mode & 0x7);
                        OldMode |= ((Mode >> 4) & 0x7) << 3;
                        OldMode |= ((Mode >> 8) & 0x7) << 6;

                        F.Mode = OldMode;
                        return true;
                    }
                    else
                        return false;
                });
        }

        public string[] ChOwn(string Filename, UInt32 Uid)
        {
            return ProcessFiles(Filename, (F, List) => { F.UserId = Uid; return true; });
        }

        public string[] ChGroup(string Filename, UInt32 Gid)
        {
            return ProcessFiles(Filename, (F, List) => { F.GroupId = Gid; return true; });
        }

        public void AddDir(string Filename, string LocalPath)
        {
            Files.Add(new CpioFile(Filename, LocalPath, true));
        }

        public void AddFile(string Filename, string LocalPath)
        {
            Files.Add(new CpioFile(Filename, LocalPath, false));
        }

        public void UpdateFile(string Filename, string LocalPath)
        {
            var F = GetFile(Filename);

            if(F != null)
            {
                var Raw = File.ReadAllBytes(LocalPath);
                var NF = F.UpdateContent(Raw);

                for(int i = 0; i < Files.Count; i++)
                {
                    if (Files[i].Path == Filename)
                        Files[i] = NF;
                }
            }
            else
            {
                // Add
                
            }
        }
    }
}
