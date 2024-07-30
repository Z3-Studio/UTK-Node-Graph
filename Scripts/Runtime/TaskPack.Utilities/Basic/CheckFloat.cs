using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class CheckFloat : ConditionTask 
    {
        public Parameter<float> firstParameter;
        public Parameter<float> secondParameter;
        public CompareMethod compare = CompareMethod.EqualTo;

        public override string Info => $"{firstParameter} {compare.GetString()} {secondParameter}";

        public override bool CheckCondition()
        {
            return compare.Compare(firstParameter, secondParameter);
        }
    }
}