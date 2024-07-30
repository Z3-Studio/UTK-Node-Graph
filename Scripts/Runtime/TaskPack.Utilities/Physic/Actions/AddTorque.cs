using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set the torque of a Rigidbody.")]
    public class AddTorque : ActionTask<Rigidbody>
    {
        public Parameter<Vector3> torque;
        public Parameter<ForceMode> forceMode;

        public override string Info => $"Add Torque = {torque}";

        protected override void StartAction()
        {
            Agent.AddTorque(torque.Value, forceMode.Value);
            EndAction(true);
        }        
    }
}