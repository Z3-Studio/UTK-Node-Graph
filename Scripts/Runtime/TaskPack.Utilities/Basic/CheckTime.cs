using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckTime : ConditionTask
    {
        [SerializeField] private Parameter<float> parameter;
        public CompareMethod compare = CompareMethod.EqualTo;

        public override string Info => $"{parameter} {compare.GetString()} Time.time";

        public override bool CheckCondition()
        {
            return compare.Compare(parameter, Time.time);
        }
    }
}