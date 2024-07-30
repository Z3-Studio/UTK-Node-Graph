using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.Exit)]
    public class FinishState : TransitableStateNode
    {
        [SerializeField] private Parameter<bool> success = true;

        public override string SubInfo => $"Return {success}";

        public override string ClassStyle => "finish-state";

        public override void UpdateState()
        {
            GraphController.ExecutionEnd(success.Value);
        }
    }
}
