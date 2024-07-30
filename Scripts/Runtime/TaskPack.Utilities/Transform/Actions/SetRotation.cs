using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.position")]
    public class SetRotation : ActionTask<Transform> {

        public Parameter<Quaternion> rotation = Quaternion.identity;

        public override string Info => $"Rotation = {rotation}";

        protected override void StartAction() {
            Agent.rotation = rotation.Value;
            EndAction(true);
        }
    }
}