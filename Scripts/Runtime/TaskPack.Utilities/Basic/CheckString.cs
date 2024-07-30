using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckString : ConditionTask
    {
        [SerializeField] private Parameter<string> firstParameter;
        [SerializeField] private Parameter<string> secondParameter;

        public override string Info => $"{firstParameter} == {secondParameter}";

        public override bool CheckCondition()
        {
            return firstParameter.Value == secondParameter.Value;
        }
    }
}
