using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Get Character Controller Velocity")]
    public class GetCharacterControllerVelocity : ActionTask<CharacterController>
    {
        public Parameter<Vector3> velocity;

        public override string Info => $"Get {AgentInfo} Velocity";

        protected override void StartAction()
        {
            velocity.Value = Agent.velocity;
            EndAction();
        }
    }
}