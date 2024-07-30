using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Compares how many children a transform has")]
    public class CheckChildCount : ConditionTask<Transform> {

        public CompareMethod checkType = CompareMethod.EqualTo;
        public Parameter<int> value;

        public override string Info
        {
            get { return "Child Count" + checkType.GetString() + value; }
        }

        public override bool CheckCondition()
        {
            return checkType.Compare(Agent.childCount, value.Value);
        }
    }
}