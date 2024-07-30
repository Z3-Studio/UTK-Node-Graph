using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    public abstract class TaskListNode<T> : LeafNode where T: Task
    {
        public abstract TaskList<T> TaskList { get; }

        public override string SubInfo => TaskList.GetInfo();

        protected sealed override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            TaskList.SetupDependencies(instances);
        }
    }
}
