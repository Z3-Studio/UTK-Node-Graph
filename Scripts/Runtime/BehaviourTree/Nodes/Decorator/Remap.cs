using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Remap)]
    public class Remap : DecoratorNode
    {
        [SerializeField] private StateResult successResult = StateResult.Failure;
        [SerializeField] private StateResult failureResult = StateResult.Success;

        public override string SubInfo => 
            $"{State.Success} -> {successResult.ToStringBold()}\n" + 
            $"{State.Failure} -> {failureResult.ToStringBold()}";

        protected override State UpdateNode()
        {
            State state = child.Update();

            return state switch
            {
                State.Success => successResult == StateResult.Success ? State.Success : State.Failure,
                State.Failure => failureResult == StateResult.Success ? State.Success : State.Failure,
                _ => state
            };
        }
    }
}
