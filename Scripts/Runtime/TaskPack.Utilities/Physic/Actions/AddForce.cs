using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Add Force")]
    public class AddForce : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;
        [SerializeField] private Parameter<Vector3> force;
        [SerializeField] private Parameter<ForceMode> forceMode = ForceMode.Force;

        public override string Info => $"Add Force = {force}, {forceMode}";

        protected override void StartAction()
        {
            data.Value.AddForce(force.Value, forceMode.Value);
            EndAction();
        }
    }
}