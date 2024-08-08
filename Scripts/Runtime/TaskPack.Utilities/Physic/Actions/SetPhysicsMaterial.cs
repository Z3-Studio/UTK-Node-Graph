using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Change the PhysicsMaterial of a Rigidbody.")]
    public class SetPhysicsMaterial : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Collider> data;
        [SerializeField] private Parameter<PhysicMaterial> physicsMaterial;

        public override string Info => $"Set PhysicsMaterial to {physicsMaterial}";

        protected override void StartAction()
        {
            data.Value.sharedMaterial = physicsMaterial.Value;
            EndAction();
        }
    }
}