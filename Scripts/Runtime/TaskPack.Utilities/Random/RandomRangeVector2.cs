using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Return float inside of range")]
    public class RandomRangeVector2 : ActionTask
    {
        [SerializeField] private Parameter<Vector2> range;
        [SerializeField] private Parameter<float> result;

        public override string Info => $"{result} = Random.Range({range})";

        protected override void StartAction()
        {
            result.Value = Random.Range(range.Value.x, range.Value.y);
            EndAction();
        }
    }
}
