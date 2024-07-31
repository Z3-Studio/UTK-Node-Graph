using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Wait a time and return successful")]
    public class WaitDelay : ActionTask
    {
        public Parameter<float> waitTime = 1f;

        public override string Info => $"Wait {waitTime} seconds";

        protected override void UpdateAction()
        {
            if (NodeRunningTime >= waitTime.Value)
            {
                EndAction();
            }
        }
    }
}
