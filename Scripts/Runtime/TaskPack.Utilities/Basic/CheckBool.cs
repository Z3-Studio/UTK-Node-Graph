using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckBool : ConditionTask
    {
        [SerializeField] private Parameter<bool> firstParameter;
        [SerializeField] private Parameter<bool> secondParameter;

        public override string Info => $"{firstParameter} == {secondParameter}";

        public override bool CheckCondition()
        {
            return firstParameter.Value == secondParameter.Value;
        }
    }
}
