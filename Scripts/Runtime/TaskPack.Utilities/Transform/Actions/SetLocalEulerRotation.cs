using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.localRotation using Euler")]
    public class SetLocalEulerRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> rotation;

        public override string Info => $"Euler Local Rotation = {rotation}";

        protected override void StartAction()
        {
            transform.Value.localRotation = Quaternion.Euler(rotation.Value);
            EndAction();
        }
    }
}