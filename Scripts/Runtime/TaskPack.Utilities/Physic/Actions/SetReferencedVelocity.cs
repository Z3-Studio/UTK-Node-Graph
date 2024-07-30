using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities 
{    
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity based on Transform direction")]
    public class SetReferencedVelocity : ActionTask<Rigidbody> 
    {        
        /*[RequiredField]*/ public Parameter<Vector3> velocity;
        public override string Info => $"Referenced velocity = {velocity}";
        protected override void StartAction() 
        {
            Agent.velocity = new Vector3()
            {
                x = Agent.transform.right.x * velocity.Value.x,
                y = Agent.transform.up.y * velocity.Value.y,
                z = Agent.transform.forward.z * velocity.Value.z
            };
            EndAction(true);
        }
    }
}