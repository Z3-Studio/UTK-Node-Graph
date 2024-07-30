using System;

namespace Z3.NodeGraph.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapToAttribute : Attribute 
    {
        public string Parameter { get; }

        public MapToAttribute(string parameter)
        {
            Parameter = parameter;
        }
    }
}
