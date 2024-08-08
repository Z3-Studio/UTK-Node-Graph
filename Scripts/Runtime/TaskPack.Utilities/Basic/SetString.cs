using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class SetString : ActionTask
    {
        [SerializeField] private Parameter<string> valueA;
        [SerializeField] private Parameter<string> valueB;

        public override string Info => $"{valueA} = {valueB}";

        protected override void StartAction()
        {
            valueA.Value = valueB.Value;
            EndAction();
        }
    }
}