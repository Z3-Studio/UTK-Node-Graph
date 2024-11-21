using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position.")]
    public class MoveToward : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> targetPosition;
        [SerializeField] private Parameter<float> speed;

        public override string Info => $"Move To {targetPosition}";

        private const float ThresholdDistance = 0.02f;

        protected override void UpdateAction() 
        {
            transform.Value.position = Vector3.MoveTowards(transform.Value.position, targetPosition.Value, DeltaTime * speed.Value);

            if (Vector3.Distance(transform.Value.position, targetPosition.Value) < ThresholdDistance) 
            {
                EndAction();
            }
        }
    }
}