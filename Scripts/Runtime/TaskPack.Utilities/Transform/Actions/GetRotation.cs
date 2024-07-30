using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get the transform.rotation.")]
    public class GetRotation : ActionTask<Transform>
    {
        [Header("Out")]
        public Parameter<Quaternion> rotation;

        public override string Info => $"Get {AgentInfo} Rotation";
        protected override void StartAction()
        {
            rotation.Value = Agent.rotation;
            EndAction(true);
        }
    }
}