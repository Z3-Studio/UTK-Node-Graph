using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.rotation using Euler")]
    public class SetEulerRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> rotation;

        public override string Info => $"Euler Rotation = {rotation}";

        protected override void StartAction()
        {
            data.Value.rotation = Quaternion.Euler(rotation.Value);
            EndAction();
        }
    }
}