using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.ConditionTask)]
    public class ConditionLeaf : TaskListNode<ConditionTask>
    {
        [SerializeField] private ConditionTaskList conditions = new();

        public override TaskList<ConditionTask> TaskList => conditions;

        protected sealed override void StartNode()
        {
            conditions.StartTaskList();
        }

        protected sealed override State UpdateNode()
        {
            return conditions.CheckConditions() ? State.Success : State.Failure;
        }

        protected sealed override void StopNode()
        {
            conditions.StopTaskList();
        }
    }
}
