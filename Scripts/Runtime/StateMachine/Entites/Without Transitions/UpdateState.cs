using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.Repeater)]
    public class UpdateState : ParallelTaskList
    {
        [DesignOnly]
        [SerializeField] private ParallelUpdateMode updateMode;

        public override ParallelExecution ParallelExecution => updateMode == ParallelUpdateMode.BeforeUpdate ? ParallelExecution.BeforeUpdate : ParallelExecution.AfterUpdate;

        public override void UpdateParallel()
        {
            taskList.Update();
            State = State.Running;
        }
    }
}
