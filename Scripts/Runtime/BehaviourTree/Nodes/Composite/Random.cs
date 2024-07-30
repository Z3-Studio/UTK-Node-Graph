using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Random)]
    public class Random : CompositeNode
    {
        // Execution: All, One
        // Can repeat same node?
        // Shuffle mode: Always, Once

        private List<BehaviourTreeNode> newList;
        private int currentChild;

        protected override void StartNode()
        {
            newList = children.ShuffleToList();
            currentChild = 0;
        }

        protected override State UpdateNode()
        {
            State state = newList[currentChild].Update();

            if (state != State.Success)
                return state;

            currentChild++;

            return currentChild >= newList.Count ? State.Success : State.Running;
        }
    }
}
