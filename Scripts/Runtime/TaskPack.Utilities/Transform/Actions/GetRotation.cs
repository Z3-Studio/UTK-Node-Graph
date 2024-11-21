using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get the transform.rotation.")]
    public class GetRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [Header("Out")]
        [SerializeField] private Parameter<Quaternion> rotation;

        public override string Info => $"Get {transform} Rotation";
        protected override void StartAction()
        {
            rotation.Value = transform.Value.rotation;
            EndAction();
        }
    }
}