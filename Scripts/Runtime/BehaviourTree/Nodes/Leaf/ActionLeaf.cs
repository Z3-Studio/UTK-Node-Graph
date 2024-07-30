using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.ActionTask)]
    public class ActionLeaf : TaskListNode<ActionTask>
    {
        [SerializeField] protected ExecutionPolicy executionPolicy;
        [SerializeField] private ActionTaskList taskList = new();

        public override TaskList<ActionTask> TaskList => taskList;
        public override string Info => "Action List";

        protected sealed override void StartNode()
        {
            taskList.StartTaskList(executionPolicy);
        }
        
        protected sealed override State UpdateNode()
        {
            return taskList.Update();
        }

        protected override void StopNode()
        {
            taskList.StopTaskList();
        }
    }
}
