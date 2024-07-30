using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target from the Agent position.")]
    public class MoveTowardReferenced : ActionTask<Transform> 
    {
        public Parameter<Vector3> targetPosition;
        public Parameter<float> speed;

        private Vector3 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => $"Move Referenced {targetPosition}";

        protected override void StartAction() 
        {
            target = new Vector3()
            {
                x = Agent.right.x * targetPosition.Value.x + Agent.position.x,
                y = Agent.up.y * targetPosition.Value.y + Agent.position.y,
                z = Agent.forward.z * targetPosition.Value.z + Agent.position.z
            };
        }

        protected override void UpdateAction() 
        {
            Agent.position = Vector3.MoveTowards(Agent.position, target, Time.fixedDeltaTime * speed.Value);

            if (Vector3.Distance(Agent.position, target) < ThresholdDistance) 
            {
                EndAction(true);
            }
        }
    }
}