using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Wait frames and return successful")]
    public class WaitDelayFrame : ActionTask
    {
        [SerializeField] private Parameter<int> frames = 1;

        public override string Info => $"Wait {frames} frames";

        private int startFrame;

        protected override void StartAction()
        {
            startFrame = Time.frameCount;
        }

        protected override void UpdateAction()
        {
            if (Time.frameCount - startFrame >= frames.Value)
            {
                EndAction();
            }
        }
    }
}
