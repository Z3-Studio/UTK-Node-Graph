using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Get Rigidbody Velocity")]
    public class GetRigidbodyVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [SerializeField] private Parameter<Vector3> velocity;

        public override string Info => $"Get {rigidbody} Velocity";
        protected override void StartAction() 
        {
            velocity.Value = rigidbody.Value.linearVelocity;
            EndAction();
        }
    }
}