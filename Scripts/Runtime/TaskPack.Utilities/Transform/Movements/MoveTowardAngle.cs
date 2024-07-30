using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position.")]
    public class MoveTowardAngle : ActionTask<Transform> {

        public Parameter<Axis3> axis = Axis3.Z;
        public Parameter<float> speed;
        public Parameter<float> angle;
        public Parameter<float> distance;

        private Vector2 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => Agent != null ? $"Move To {angle}º" : name;
        private Vector2 Target => MathUtils.AngleToDirection(angle.Value, distance.Value) + (Vector2)Agent.position;

        protected override void StartAction() {
            target = Target;
        }


        protected override void UpdateAction() {

            Agent.position = Vector3.MoveTowards(Agent.position, target, Time.fixedDeltaTime * speed.Value);

            if (Vector3.Distance(Agent.position, target) < ThresholdDistance) {
                Agent.position = target;
                EndAction(true);
            }
        }
    }
}