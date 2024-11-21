using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target from the Agent position.")]
    public class MoveTowardReferenced : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> targetPosition;
        [SerializeField] private Parameter<float> speed;

        private Vector3 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => $"Move Referenced {targetPosition}";

        protected override void StartAction() 
        {
            target = new Vector3()
            {
                x = transform.Value.right.x * targetPosition.Value.x + transform.Value.position.x,
                y = transform.Value.up.y * targetPosition.Value.y + transform.Value.position.y,
                z = transform.Value.forward.z * targetPosition.Value.z + transform.Value.position.z
            };
        }

        protected override void UpdateAction() 
        {
            transform.Value.position = Vector3.MoveTowards(transform.Value.position, target, DeltaTime * speed.Value);

            if (Vector3.Distance(transform.Value.position, target) < ThresholdDistance) 
            {
                EndAction();
            }
        }
    }
}