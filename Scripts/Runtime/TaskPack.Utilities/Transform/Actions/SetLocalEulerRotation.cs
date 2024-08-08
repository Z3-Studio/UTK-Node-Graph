using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.localRotation using Euler")]
    public class SetLocalEulerRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> rotation;

        public override string Info => $"Euler Local Rotation = {rotation}";

        protected override void StartAction()
        {
            data.Value.localRotation = Quaternion.Euler(rotation.Value);
            EndAction();
        }
    }
}