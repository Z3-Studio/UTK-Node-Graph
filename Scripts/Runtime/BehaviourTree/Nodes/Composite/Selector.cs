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
            while (currentChild < children.Count)
            {
                State state = children[currentChild].Update();

                if (state != State.Failure)
                    return state;

                currentChild++;
            }

            return State.Failure;
        }
    }
}
