using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities 
{    
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity based on Transform direction")]
    public class SetReferencedVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [SerializeField] private Parameter<Vector3> velocity;
        public override string Info => $"Referenced velocity = {velocity}";
        protected override void StartAction() 
        {
            rigidbody.Value.linearVelocity = new Vector3()
            {
                x = rigidbody.Value.transform.right.x * velocity.Value.x,
                y = rigidbody.Value.transform.up.y * velocity.Value.y,
                z = rigidbody.Value.transform.forward.z * velocity.Value.z
            };
            EndAction();
        }
    }
}