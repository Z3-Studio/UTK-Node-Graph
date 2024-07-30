using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Moves a rigidbody in the Transform.Forward direction. This movement preserves the current speed in Y.")]
    public class ForwardMovement : ActionTask<Rigidbody>
    {
        public Parameter<float> speed;
        public override string Info => $"Forward Movement = {speed}";
        protected override void StartAction()
        {
            Vector3 forward = Agent.transform.forward * speed.Value;
            Agent.velocity = new Vector3(forward.x, Agent.velocity.y, forward.z);
            EndAction(true);
        }
    }
}