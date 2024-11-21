using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get Character Controller Velocity")]
    public class InverseTransformDirection : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> direction;
        [SerializeField] private Parameter<Vector3> inverse;

        public override string Info => $"Get {transform} Velocity";

        protected override void StartAction()
        {
            inverse.Value = transform.Value.InverseTransformDirection(direction.Value);
            EndAction();
        }
    }
}