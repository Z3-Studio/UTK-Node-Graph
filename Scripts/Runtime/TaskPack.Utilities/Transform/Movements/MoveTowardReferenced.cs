using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target from the Agent position.")]
    public class MoveTowardReferenced : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> targetPosition;
        [SerializeField] private Parameter<float> speed;

        private Vector3 target;
        private const float ThresholdDistance = 0.02f;

        public override string Info => $"Move Referenced {targetPosition}";

        protected override void StartAction() 
        {
            target = new Vector3()
            {
                x = data.Value.right.x * targetPosition.Value.x + data.Value.position.x,
                y = data.Value.up.y * targetPosition.Value.y + data.Value.position.y,
                z = data.Value.forward.z * targetPosition.Value.z + data.Value.position.z
            };
        }

        protected override void UpdateAction() 
        {
            data.Value.position = Vector3.MoveTowards(data.Value.position, target, DeltaTime * speed.Value);

            if (Vector3.Distance(data.Value.position, target) < ThresholdDistance) 
            {
                EndAction();
            }
        }
    }
}