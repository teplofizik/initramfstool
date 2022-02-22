using System;
using System.Collections.Generic;
using System.IO;
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

        public void Delete(string Filename)
        {
            Files.RemoveAll(F => (F.Path == Filename));
        }

        public void ChMod(string Filename, UInt32 Mode)
        {
            var F = GetFile(Filename);
            if(F != null)
            {
                // rwxr-x---
                var OldMode = F.Mode & ~0x1FFU;

                OldMode |= (Mode & 0x7);
                OldMode |= ((Mode >> 4) & 0x7) << 3;
                OldMode |= ((Mode >> 8) & 0x7) << 6;

                F.Mode = OldMode;
            }
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
