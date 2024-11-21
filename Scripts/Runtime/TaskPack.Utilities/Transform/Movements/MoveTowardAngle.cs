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
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Axis3> axis = Axis3.Z;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<float> angle;
        [SerializeField] private Parameter<float> distance;

        private Vector2 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => $"Move To {angle} Angleº";
        private Vector2 Target => MathUtils.AngleToDirection(angle.Value, distance.Value) + (Vector2)transform.Value.position;

        protected override void StartAction()
        {
            target = Target;
        }

        protected override void UpdateAction()
        {
            transform.Value.position = Vector3.MoveTowards(transform.Value.position, target, DeltaTime * speed.Value);

            if (Vector3.Distance(transform.Value.position, target) < ThresholdDistance)
            {
                transform.Value.position = target;
                EndAction();
            }
        }
    }
}