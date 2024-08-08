using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set the torque of a Rigidbody.")]
    public class AddTorque : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;
        [SerializeField] private Parameter<Vector3> torque;
        [SerializeField] private Parameter<ForceMode> forceMode;

        public override string Info => $"Add Torque = {torque}";

        protected override void StartAction()
        {
            data.Value.AddTorque(torque.Value, forceMode.Value);
            EndAction();
        }        
    }
}