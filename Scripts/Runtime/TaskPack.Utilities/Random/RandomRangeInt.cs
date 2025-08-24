using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Return int inside of range")]
    public class RandomRangeInt : ActionTask
    {
        [SerializeField] private Parameter<int> minInclusive;
        [SerializeField] private Parameter<int> maxExclusie;
        [SerializeField] private Parameter<int> result;

        public override string Info => $"{result} = Random.Range({minInclusive}, {maxExclusie})";

        protected override void StartAction()
        {
            result.Value = Random.Range(minInclusive.Value, maxExclusie.Value);
            EndAction();
        }
    }
}
