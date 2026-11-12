using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Wait a time and return successful")]
    public class CheckDelay : ConditionTask
    {
        [SerializeField] private Parameter<float> waitTime = 1f;

        public override string InfoC => $"Wait {waitTime} seconds";

        private float NodeRunningTime { get; set; }

        public override void StartCondition()
        {
            NodeRunningTime = 0;
        }

        public override bool CheckCondition()
        {
            NodeRunningTime += DeltaTime;
            return NodeRunningTime >= waitTime.Value;
        }
    }
}
