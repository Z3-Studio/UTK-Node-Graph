using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position.")]
    public class MoveToward : ActionTask<Transform> 
    {
        public Parameter<Vector3> targetPosition;
        public Parameter<float> speed;

        public override string Info => $"Move To {targetPosition}";

        private const float ThresholdDistance = 0.02f;

        protected override void UpdateAction() 
        {
            Agent.position = Vector3.MoveTowards(Agent.position, targetPosition.Value, Time.fixedDeltaTime * speed.Value);

            if (Vector3.Distance(Agent.position, targetPosition.Value) < ThresholdDistance) 
            {
                EndAction();
            }
        }
    }
}