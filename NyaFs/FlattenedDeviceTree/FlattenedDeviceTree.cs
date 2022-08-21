using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.FlattenedDeviceTree
{
    public class FlattenedDeviceTree
    {
        public Types.Node Root = new Types.Node();


        public uint CpuId = 0;

        /// <summary>
        /// Зарезре
        /// </summary>
        public Types.ReservedMemory[] ReserveMemory = new Types.ReservedMemory[] { };
    }
}
