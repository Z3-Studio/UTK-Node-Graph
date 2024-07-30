using System;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to rename Tasks and Nodes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NgNameAttribute : Attribute
    {
        public string Name { get; }

        public NgNameAttribute(string name)
        {
            Name = name;
        }
    }
}