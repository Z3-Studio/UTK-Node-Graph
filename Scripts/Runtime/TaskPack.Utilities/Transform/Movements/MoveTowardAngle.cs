using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position.")]
    public class MoveTowardAngle : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Axis3> axis = Axis3.Z;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<float> angle;
        [SerializeField] private Parameter<float> distance;

        private Vector2 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => $"Move To {angle} Angleº";
        private Vector2 Target => MathUtils.AngleToDirection(angle.Value, distance.Value) + (Vector2)data.Value.position;

        protected override void StartAction()
        {
            target = Target;
        }

        protected override void UpdateAction()
        {
            data.Value.position = Vector3.MoveTowards(data.Value.position, target, Time.fixedDeltaTime * speed.Value);

            if (Vector3.Distance(data.Value.position, target) < ThresholdDistance)
            {
                data.Value.position = target;
                EndAction();
            }
        }
    }
}