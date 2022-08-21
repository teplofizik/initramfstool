using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.FlattenedDeviceTree.Types
{
    public class Node
    {
        /// <summary>
        /// Node name
        /// </summary>
        public string Name;

        /// <summary>
        /// List of properties
        /// </summary>
        public List<Property> Properties = new List<Property>();

        /// <summary>
        /// List of nested nodes
        /// </summary>
        public List<Node> Nodes = new List<Node>();

        public override string ToString()
        {
            return $"NODE {Name} P:{Properties.Count} N:{Nodes.Count}";
        }
    }
}
