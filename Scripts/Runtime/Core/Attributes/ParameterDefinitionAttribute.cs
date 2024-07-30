using System;

namespace Z3.NodeGraph.Core
{
    public enum AutoBindType
    {
        None,
        SelfBind,
        FindSameVariable,
        FindSimilarVariable
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class ParameterDefinitionAttribute : Attribute 
    {
        public bool Get { get; }
        public bool Set { get; }
        public AutoBindType AutoBindType { get; }

        public ParameterDefinitionAttribute(AutoBindType autoBindType = AutoBindType.None, bool get = true, bool set = true)
        {
            AutoBindType = autoBindType;
            Get = get;
            Set = set;
        }
    }
}
