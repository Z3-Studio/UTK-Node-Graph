using System;

namespace Z3.NodeGraph.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeIconAttribute : Attribute
    {
        public string Path { get; }
        public GraphIcon IconType { get; }

        public NodeIconAttribute(GraphIcon icon)
        {
            IconType = icon;
        }
    }

    public enum GraphIcon
    {
        None,
        GraphRunner,
        SubGraph,
        Random,
        Selector,
        Sequencer,
        ConditionTask,
        ActionTask,
        Parallel,
        Interruptor,
        Repeater,
        Remap,
        Invert,
        ForEach,
        Exit,
        Enter,
        WaitUntil,
        TimeOut
    }
}
