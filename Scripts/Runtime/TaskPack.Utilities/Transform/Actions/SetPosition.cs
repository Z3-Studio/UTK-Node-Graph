using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.position")]
    public class SetPosition : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> position;

        public override string Info => $"Position = {position}";

        protected override void StartAction()
        {
            data.Value.position = position.Value;
            EndAction();
        }
    }
}