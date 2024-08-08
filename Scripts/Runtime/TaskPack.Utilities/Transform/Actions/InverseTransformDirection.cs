using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get Character Controller Velocity")]
    public class InverseTransformDirection : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> direction;
        [SerializeField] private Parameter<Vector3> inverse;

        public override string Info => $"Get {data} Velocity";

        protected override void StartAction()
        {
            inverse.Value = data.Value.InverseTransformDirection(direction.Value);
            EndAction();
        }
    }
}