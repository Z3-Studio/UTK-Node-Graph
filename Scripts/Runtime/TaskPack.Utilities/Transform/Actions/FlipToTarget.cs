using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Change the Y rotation based on target")]
    public class FlipToTarget : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;

        public override string Info => $"Flip To {target}";

        protected override void StartAction() {
            LookAtTarget();
            EndAction();
        }

        private void LookAtTarget() {
            bool lookingRight = transform.Value.right.x > 0;
            bool targetRight = transform.Value.position.x < target.Value.x;

            if (lookingRight != targetRight) {
                transform.Value.Rotate(0f, 180f, 0f);
            }
        }
    }
}