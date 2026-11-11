using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [ClassStyle("finish-state")]
    [NodeIcon(GraphIcon.Exit)]
    public class FinishState : TransitableStateNode
    {
        [SerializeField] private Parameter<bool> success = true;

        public override string SubInfo => $"Return {success}";

        public override void StartState()
        {
            base.StartState();
            State = State.Ready;
        }

        public override void UpdateState()
        {
            State = success.Value ? State.Success : State.Failure;
            GraphController.ExecutionEnd(State);
        }
    }
}
