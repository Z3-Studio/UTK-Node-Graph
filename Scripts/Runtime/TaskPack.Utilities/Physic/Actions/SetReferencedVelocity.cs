using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities 
{    
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity based on Transform direction")]
    public class SetReferencedVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;
        [SerializeField] private Parameter<Vector3> velocity;
        public override string Info => $"Referenced velocity = {velocity}";
        protected override void StartAction() 
        {
            data.Value.velocity = new Vector3()
            {
                x = data.Value.transform.right.x * velocity.Value.x,
                y = data.Value.transform.up.y * velocity.Value.y,
                z = data.Value.transform.forward.z * velocity.Value.z
            };
            EndAction();
        }
    }
}