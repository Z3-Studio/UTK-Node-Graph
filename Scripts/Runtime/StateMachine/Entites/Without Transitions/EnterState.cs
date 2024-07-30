using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.Enter)]
    public class EnterState : ParallelTaskList
    {
        public override ParallelExecution ParallelExecution => ParallelExecution.Enter;

        private bool finish;

        public override void StartGraph()
        {
            base.StartGraph();
            finish = false;
        }

        public override void UpdateParallel()
        {
            if (finish)
                return;

            State = taskList.Update();

            if (State != State.Running)
            {
                finish = true;
            }
        }
    }
}
