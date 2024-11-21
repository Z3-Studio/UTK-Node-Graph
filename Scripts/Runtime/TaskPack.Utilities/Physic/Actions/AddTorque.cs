using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set the torque of a Rigidbody.")]
    public class AddTorque : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [SerializeField] private Parameter<Vector3> torque;
        [SerializeField] private Parameter<ForceMode> forceMode;

        public override string Info => $"Add Torque = {torque}";

        protected override void StartAction()
        {
            rigidbody.Value.AddTorque(torque.Value, forceMode.Value);
            EndAction();
        }        
    }
}