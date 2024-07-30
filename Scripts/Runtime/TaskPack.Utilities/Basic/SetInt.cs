using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class SetInt : ActionTask 
    {
        public OperationMethod operation = OperationMethod.Add;
        public Parameter<int> valueA;
        public Parameter<int> valueB = 1;

        public override string Info => $"{valueA} {operation.GetString()} {valueB}";

        protected override void StartAction()
        {
            valueA.Value = operation.Operate(valueA.Value, valueB.Value);
            EndAction(true);
        }
    }
}