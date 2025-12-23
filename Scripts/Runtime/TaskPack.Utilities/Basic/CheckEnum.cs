using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckEnum : ConditionTask
    {
        [SerializeField] private Parameter<Enum> firstParameter;
        //[SerializeField] private GenericParameter<int> secondParameter;
        [SerializeField] private Parameter<int> secondParameter;

        public override string InfoC => $"{firstParameter} == {secondParameter}";

        public override bool CheckCondition()
        {
            int intValue = Convert.ToInt32(firstParameter.Value);
            return intValue == secondParameter.Value;
        }
    }
}
