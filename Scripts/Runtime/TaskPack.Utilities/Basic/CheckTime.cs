using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckTime : ConditionTask
    {
        [SerializeField] private Parameter<float> parameter;
        [SerializeField] private Parameter<float> extra;
        public CompareMethod compare = CompareMethod.EqualTo;

        public override string InfoC => $"Time.time {compare.GetString()} {parameter} {(extra.IsBinding ? extra : extra.Value == 0 ? string.Empty : $"+ ({extra})")}";     

        public override bool CheckCondition()
        {
            return compare.Compare(Time.time, parameter + extra);
        }
    }
}