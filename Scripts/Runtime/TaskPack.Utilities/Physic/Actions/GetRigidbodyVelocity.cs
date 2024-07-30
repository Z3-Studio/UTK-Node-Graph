using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Get Rigidbody Velocity")]
    public class GetRigidbodyVelocity : ActionTask<Rigidbody>
    {
        public Parameter<Vector3> velocity;

        public override string Info => $"Get {AgentInfo} Velocity";
        protected override void StartAction() 
        {
            velocity.Value = Agent.velocity;
            EndAction(true);
        }
    }
}