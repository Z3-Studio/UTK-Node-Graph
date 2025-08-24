using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Return int inside of range")]
    public class RandomRangeVector2Int : ActionTask
    {
        [SerializeField] private Parameter<Vector2Int> range;
        [SerializeField] private Parameter<int> result;

        public override string Info => $"{result} = Random.Range({range})";

        protected override void StartAction()
        {
            result.Value = Random.Range(range.Value.x, range.Value.y);
            EndAction();
        }
    }
}
