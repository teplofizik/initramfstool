using System;
using System.Collections.Generic;
using System.Text;

namespace CpioLib.Types
{
    public enum CpioModeFileType {
        C_ISDIR = 0040000, // Directory
        C_ISFIFO = 0010000, // FIFO
        C_ISREG = 0100000, // Regular file
        C_ISBLK = 0060000, // Block special
        C_ISCHR = 0020000, // Character special
        C_ISCTG = 0110000, // Reserved
        C_ISLNK = 0120000, // Symbolic link.
        C_ISSOCK = 0140000, // Socket
    }
}
