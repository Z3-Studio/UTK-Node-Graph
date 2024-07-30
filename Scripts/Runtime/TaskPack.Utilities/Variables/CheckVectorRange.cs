using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Best way to check if some object is null")]
    public class CheckVectorRange : ConditionTask
    {
        public enum RangeCompareMethod
        {
            Below,
            Inside,
            Above
        }

        public Parameter<float> variable;
        public Parameter<Vector2> range;
        public Parameter<RangeCompareMethod> checkType = RangeCompareMethod.Inside;

        public override string Info
        {
            get => checkType.Value switch
            {
                RangeCompareMethod.Below => $"{variable} < {range}.X",
                RangeCompareMethod.Inside => $"{variable} Inside {range}",
                RangeCompareMethod.Above => $"{variable} > {range}.Y",
                _ => throw new System.NotImplementedException(),
            };
        }

        public override bool CheckCondition()
        {
            return checkType.Value switch
            {
                RangeCompareMethod.Below => variable.Value < range.Value.x,
                RangeCompareMethod.Inside => range.Value.InsideRange(variable.Value),
                RangeCompareMethod.Above => variable.Value > range.Value.y,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}