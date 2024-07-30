using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Change the Y rotation based on target")]
    public class FlipToTarget : ActionTask<Transform> 
    {
        public Parameter<Vector3> target;

        public override string Info => $"Flip To {target}";

        protected override void StartAction() {
            LookAtTarget();
            EndAction(true);
        }

        private void LookAtTarget() {
            bool lookingRight = Agent.right.x > 0;
            bool targetRight = Agent.position.x < target.Value.x;

            if (lookingRight != targetRight) {
                Agent.Rotate(0f, 180f, 0f);
            }
        }
    }
}