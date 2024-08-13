using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position.")]
    public class MoveToward : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> targetPosition;
        [SerializeField] private Parameter<float> speed;

        public override string Info => $"Move To {targetPosition}";

        private const float ThresholdDistance = 0.02f;

        protected override void UpdateAction() 
        {
            data.Value.position = Vector3.MoveTowards(data.Value.position, targetPosition.Value, DeltaTime * speed.Value);

            if (Vector3.Distance(data.Value.position, targetPosition.Value) < ThresholdDistance) 
            {
                EndAction();
            }
        }
    }
}