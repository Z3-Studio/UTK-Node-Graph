using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Clamp a float between two values.")]
    public class ClampFloat : ActionTask
    {
        [SerializeField] private Parameter<float> value;
        [SerializeField] private Parameter<float> min;
        [SerializeField] private Parameter<float> max;

        protected override void StartAction() {
            value.Value = Mathf.Clamp(value.Value, min.Value, max.Value);
            EndAction();
        }
    }
}