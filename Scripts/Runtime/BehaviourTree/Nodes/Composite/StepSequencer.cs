using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Sequencer)]
    [NodeDescription("It executes one child per call, and when finished returns the value. On the next call it jumps to the next child repeating the process.")]
    public class StepSequencer : CompositeNode
    {
        [SerializeField] private Parameter<bool> dontStop;

        private int currentChild;

        protected override State UpdateNode()
        {
            if (currentChild >= children.Count)
            {
                currentChild = 0;
            }

            State state = children[currentChild].Update();

            if (state == State.Success || state == State.Failure)
            {
                currentChild++;
                return state;
            }

            return state;
        }
    }
}
