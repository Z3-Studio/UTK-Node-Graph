using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckInt : ConditionTask
    {
        public Parameter<int> firstParameter;
        public Parameter<int> secondParameter;
        public CompareMethod compare = CompareMethod.EqualTo;

        public override string Info => $"{firstParameter} {compare.GetString()} {secondParameter}";

        public override bool CheckCondition()
        {
            return compare.Compare(firstParameter, secondParameter);
        }
    }
}