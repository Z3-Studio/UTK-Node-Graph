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

#if UNITY_6000_0_OR_NEWER
        [SerializeField] private Parameter<PhysicsMaterial> physicsMaterial;
#else
        [SerializeField] private Parameter<PhysicMaterial> physicsMaterial;
#endif

        public override string Info => $"Set PhysicsMaterial to {physicsMaterial}";

        protected override void StartAction()
        {
            collider.Value.sharedMaterial = physicsMaterial.Value;
            EndAction();
        }
    }
}