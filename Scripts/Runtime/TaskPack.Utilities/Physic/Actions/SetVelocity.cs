using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity")]
    public class SetVelocity : ActionTask<Rigidbody> {

        public Parameter<Vector3> velocity;
        public override string Info => $"Velocity = {velocity}";
        protected override void StartAction() {
            Agent.velocity = velocity.Value;
            EndAction(true);
        }
    }
}