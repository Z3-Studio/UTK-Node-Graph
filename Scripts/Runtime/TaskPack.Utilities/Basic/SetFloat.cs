using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class SetFloat :  SetOperation<float>
    {
        protected override void StartAction()
        {
            valueA.Value = operation.Operate(valueA.Value, valueB.Value);
            EndAction();
        }
    }
}
