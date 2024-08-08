using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Variables)]
    [NodeDescription("Return a random value between range.x and range.y")]
    public class RandomRange : ActionTask {

        [Header("In")]
        [SerializeField] private Parameter<Vector2> range;

        [Header("Out")]
        [SerializeField] private Parameter<float> result;

        public override string Info => range.IsBinding ?
            $"Random Range {range}" :
            $"Random ({range.Value.x} - {range.Value.y})";

        protected override void StartAction() {
            result.Value = range.Value.RandomRange();
            EndAction();
        }
    }
}