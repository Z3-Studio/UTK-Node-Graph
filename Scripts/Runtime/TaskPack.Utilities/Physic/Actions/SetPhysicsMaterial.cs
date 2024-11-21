using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Change the PhysicsMaterial of a Rigidbody.")]
    public class SetPhysicsMaterial : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Collider> collider;
        [SerializeField] private Parameter<PhysicsMaterial> physicsMaterial;

        public override string Info => $"Set PhysicsMaterial to {physicsMaterial}";

        protected override void StartAction()
        {
            collider.Value.sharedMaterial = physicsMaterial.Value;
            EndAction();
        }
    }
}