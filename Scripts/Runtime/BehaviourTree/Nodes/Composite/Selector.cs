using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Selector)]
    public class Selector : CompositeNode
    {
        private int currentChild;

        protected override void StartNode()
        {
            currentChild = 0;
        }

        protected override State UpdateNode()
        {
            State state = children[currentChild].Update();

            if (state != State.Failure)
                return state;

            currentChild++;

            return currentChild >= children.Count ? State.Failure : State.Running;
        }
    }
}
