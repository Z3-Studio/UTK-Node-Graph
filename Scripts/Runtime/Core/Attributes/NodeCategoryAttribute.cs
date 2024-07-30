using System;

namespace Z3.NodeGraph.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCategoryAttribute : Attribute
    {
        public string Path { get; }

        public NodeCategoryAttribute(string path)
        {
            Path = path;
        }
    }
}