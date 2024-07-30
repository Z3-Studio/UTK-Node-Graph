using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get Character Controller Velocity")]
    public class InverseTransformDirection : ActionTask<Transform>
    {
        public Parameter<Vector3> direction;
        public Parameter<Vector3> inverse;

        public override string Info => $"Get {AgentInfo} Velocity";

        protected override void StartAction()
        {
            inverse.Value = Agent.InverseTransformDirection(direction.Value);
            EndAction();
        }
    }
}