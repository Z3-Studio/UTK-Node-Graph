using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Get Rigidbody Velocity")]
    public class GetRigidbodyVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;
        [SerializeField] private Parameter<Vector3> velocity;

        public override string Info => $"Get {data} Velocity";
        protected override void StartAction() 
        {
            velocity.Value = data.Value.velocity;
            EndAction();
        }
    }
}