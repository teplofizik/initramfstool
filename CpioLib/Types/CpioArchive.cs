using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CpioLib.Types
{
    public class CpioArchive
    {
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
