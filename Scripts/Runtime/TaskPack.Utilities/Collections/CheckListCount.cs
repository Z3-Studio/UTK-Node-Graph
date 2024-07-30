using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System.Collections;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Collections)]
    [NodeDescription("Compare the list Count")]
    public class CheckListCount : ConditionTask {

        public Parameter<IList> list;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public Parameter<int> value;
        public override string Info {
            get { return list + ".Count" + checkType.GetString() + value; }
        }

        public override bool CheckCondition() {
            return checkType.Compare(list.Value.Count, value.Value);
        }
    }
}