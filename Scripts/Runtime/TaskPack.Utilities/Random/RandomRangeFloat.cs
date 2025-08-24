using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Return float inside of range")]
    public class RandomRangeFloat : ActionTask
    {
        [SerializeField] private Parameter<float> minInclusive;
        [SerializeField] private Parameter<float> maxExclusie;
        [SerializeField] private Parameter<float> result;

        public override string Info => $"{result} = Random.Range({minInclusive}, {maxExclusie})";

        protected override void StartAction()
        {
            result.Value = Random.Range(minInclusive.Value, maxExclusie.Value);
            EndAction();
        }
    }
}
