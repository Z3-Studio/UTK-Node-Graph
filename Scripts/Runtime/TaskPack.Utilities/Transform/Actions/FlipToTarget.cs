using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Change the Y rotation based on target")]
    public class FlipToTarget : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> target;

        public override string Info => $"Flip To {target}";

        protected override void StartAction() {
            LookAtTarget();
            EndAction();
        }

        private void LookAtTarget() {
            bool lookingRight = data.Value.right.x > 0;
            bool targetRight = data.Value.position.x < target.Value.x;

            if (lookingRight != targetRight) {
                data.Value.Rotate(0f, 180f, 0f);
            }
        }
    }
}