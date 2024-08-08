using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.position")]
    public class SetRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Quaternion> rotation = Quaternion.identity;

        public override string Info => $"Rotation = {rotation}";

        protected override void StartAction()
        {
            data.Value.rotation = rotation.Value;
            EndAction();
        }
    }
}