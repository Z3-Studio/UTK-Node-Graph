using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Invert)]
    public class Invert : DecoratorNode
    {
        protected override State UpdateNode()
        {
            State state = child.Update();

            return state switch
            {
                State.Failure => State.Success,
                State.Success => State.Failure,
                _ => state,
            };
        }
    }
}
