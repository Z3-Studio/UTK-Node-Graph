using System;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.Parallel)]
    public class Parallel : CompositeNode
    {
        public enum ParallelPolicy
        {
            FirstStop,
            FirstSuccess,
            FirstFailure,
            AllStop
        }

        public ParallelPolicy policy = ParallelPolicy.FirstStop;

        private Func<State> updateMethod;

        public override string SubInfo => $"Mode {policy.ToStringBold()}";

        protected override void StartNode()
        {
            updateMethod = policy switch
            {
                ParallelPolicy.AllStop => AllStop,
                ParallelPolicy.FirstStop => FirstStop,
                ParallelPolicy.FirstSuccess => FirstSuccess,
                ParallelPolicy.FirstFailure => FirstFailure,
                _ => throw new NotImplementedException(),
            };
        }

        protected override State UpdateNode() => updateMethod();

        private State AllStop() // Return success if all is success
        {
            throw new NotImplementedException();
        }

        private State FirstStop()
        {
            State finalResult = State.Success;

            // Parallel Execution
            foreach (BehaviourTreeNode node in children)
            {
                finalResult = node.Update();

                if (finalResult is State.Success or State.Failure)
                {
                    foreach (BehaviourTreeNode child in children)
                    {
                        child.Interrupt();
                    }
                    break;
                }
            }

            return finalResult;
        }

        private State FirstSuccess()
        {
            throw new NotImplementedException();
        }

        private State FirstFailure()
        {
            throw new NotImplementedException();
        }
    }
}
