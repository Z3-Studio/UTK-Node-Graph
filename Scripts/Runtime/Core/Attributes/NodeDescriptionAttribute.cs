using System;

namespace Z3.NodeGraph.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public NodeDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
