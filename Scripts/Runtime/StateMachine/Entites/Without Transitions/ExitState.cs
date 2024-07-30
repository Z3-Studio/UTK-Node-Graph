using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.Exit)]
    [NodeDescription("That action will only execute for 1 frame")]
    public class ExitState : ParallelTaskList
    {
        public override ParallelExecution ParallelExecution => ParallelExecution.Exit;

        public override void StartGraph()
        {
            State = State.Resting;
            taskList.StartTaskList(executionPolicy);
        }

        public override void UpdateParallel()
        {
            State = taskList.Update();
        }
    }
}
