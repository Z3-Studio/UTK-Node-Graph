using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity")]
    public class SetVelocity : ActionTask 
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;

        [SerializeField] private Parameter<Vector3> velocity;
        public override string Info => $"Velocity = {velocity}";
        protected override void StartAction() {
            data.Value.velocity = velocity.Value;
            EndAction();
        }
    }
}