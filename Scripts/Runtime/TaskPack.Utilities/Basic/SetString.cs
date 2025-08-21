using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class SetString : SetOperation<string>
    {
        protected override void StartAction()
        {
            valueA.Value = valueB.Value;
            EndAction();
        }
    }
}
