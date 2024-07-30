using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.WaitUntil)]
    public class WaitUntilLeaf : TaskListNode<ConditionTask>
    {
        [SerializeField] private ConditionTaskList conditions = new();

        public override TaskList<ConditionTask> TaskList => conditions;

        public override string Info => "Wait Until";

        protected sealed override void StartNode()
        {
            conditions.StartTaskList();
        }

        protected sealed override State UpdateNode()
        {
            bool successful = conditions.CheckConditions();
            return successful ? State.Success : State.Running;
        }

        protected sealed override void StopNode()
        {
            conditions.StopTaskList();
        }
    }
}
