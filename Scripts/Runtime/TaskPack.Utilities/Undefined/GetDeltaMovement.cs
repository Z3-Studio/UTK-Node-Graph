using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{
    [NodeCategory(Categories.Math)]
    [NodeDescription("TODO")]
    public class GetDeltaMovement : ActionTask<Transform>
    {
        public Parameter<Vector3> velocity;

        public override string Info => $"Get {AgentInfo} AI Velocity";

        private Vector3 previousPosition;
        private int previousFrame;
        protected override void StartAction()
        {
            previousFrame = Time.frameCount;
            previousPosition = Agent.position;
        }

        protected override void UpdateAction()
        {
            if (Time.frameCount > previousFrame)
            {
                velocity.Value = (Agent.position - previousPosition) / Time.fixedDeltaTime;
                EndAction();
            }
        }
    }
}