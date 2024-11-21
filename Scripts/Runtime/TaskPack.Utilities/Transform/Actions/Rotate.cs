using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("transform.Rotation(euler)")]
    public class Rotate : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> eulerAngles;

        public override string Info => $"Rotate {eulerAngles}";

        protected override void StartAction()
        {
            transform.Value.Rotate(eulerAngles.Value);
            EndAction();
        }
    }
}