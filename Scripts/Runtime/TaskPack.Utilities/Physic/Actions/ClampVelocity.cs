using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Clamp Velocity to a set of values")]
    public class ClampVelocity : ActionTask<Rigidbody>
    {
        [Header("Inputs")]
        public Parameter<Vector2> range;
        public override string Info => $"Clamp Velocity, Range: {range}";
        protected override void StartAction()
        {
            Agent.velocity = new Vector3()
            {
                x = Mathf.Clamp(Agent.velocity.x, range.Value.x, range.Value.y),
                y = Mathf.Clamp(Agent.velocity.y, range.Value.x, range.Value.y),
                z = Mathf.Clamp(Agent.velocity.z, range.Value.x, range.Value.y)
            };
            EndAction(true);
        }
    }
}