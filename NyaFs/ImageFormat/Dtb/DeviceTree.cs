using NyaFs.FlattenedDeviceTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Dtb
{
    public class DeviceTree
    {
        public FlattenedDeviceTree.FlattenedDeviceTree DevTree = new FlattenedDeviceTree.FlattenedDeviceTree();

        public DeviceTree(FlattenedDeviceTree.FlattenedDeviceTree DevTree)
        {
            this.DevTree = DevTree;
        }
    }
}
