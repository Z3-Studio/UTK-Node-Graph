using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Sequencer)]
    [NodeDescription("It executes one child per call, and when finished returns the value. On the next call it jumps to the next child repeating the process.")]
    public class StepSequencer : CompositeNode
    {
        [SerializeField] private Parameter<int> currentChild;

        protected override State UpdateNode()
        {
            if (children.Count == 0)
                return State.Success;

            if (currentChild.Value >= children.Count)
            {
                currentChild.Value = 0;
            }

            State state = children[currentChild.Value].Update();

            if (state == State.Success || state == State.Failure)
            {
                currentChild.Value++;
                return state;
            }

            return state;
        }
    }
}
