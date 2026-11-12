using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Wait frames and return successful")]
    public class CheckDelayFrame : ConditionTask
    {
        [SerializeField] private Parameter<int> frames = 1;

        public override string InfoC => $"Wait {frames} frames";

        private int startFrame;

        public override void StartCondition()
        {
            startFrame = Time.frameCount;
        }

        public override bool CheckCondition()
        {
            return Time.frameCount - startFrame >= frames.Value;
        }
    }
}
