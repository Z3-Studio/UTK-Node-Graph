using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System.Collections;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Collections)]
    [NodeDescription("Compare the list Count")]
    public class CheckListCount : ConditionTask
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private CompareMethod checkType = CompareMethod.EqualTo;
        [SerializeField] private Parameter<int> value;

        public override string Info => list + ".Count" + checkType.GetString() + value;

        public override bool CheckCondition()
        {
            return checkType.Compare(list.Value.Count, value.Value);
        }
    }
}