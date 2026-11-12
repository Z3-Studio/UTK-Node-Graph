using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Sequencer)]
    public class Sequencer : CompositeNode
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

                if (state != State.Success)
                    return state;

                currentChild++;
            }

            return State.Success;
        }
    }
}
