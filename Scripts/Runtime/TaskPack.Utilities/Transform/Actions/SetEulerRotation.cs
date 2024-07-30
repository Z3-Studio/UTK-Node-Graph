using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.rotation using Euler")]
    public class SetEulerRotation : ActionTask<Transform>
    {
        public Parameter<Vector3> rotation;

        public override string Info => $"Euler Rotation = {rotation}";

        protected override void StartAction()
        {
            Agent.rotation = Quaternion.Euler(rotation.Value);
            EndAction(true);
        }
    }
}