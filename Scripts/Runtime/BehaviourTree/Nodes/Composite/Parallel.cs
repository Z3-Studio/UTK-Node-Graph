using System;
using System.Collections.Generic;
using System.Linq;
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
            AllStop,
        }

        public ParallelPolicy policy = ParallelPolicy.FirstStop;

        public override string SubInfo => $"Mode {policy.ToStringBold()}";

        private List<BehaviourTreeNode> executionList;
        private Func<State> updateMethod;

        protected override void StartNode()
        {
            executionList = new List<BehaviourTreeNode>(children);

            updateMethod = policy switch
            {
                ParallelPolicy.AllStop => AllStop,
                ParallelPolicy.FirstStop => FirstStop,
                ParallelPolicy.FirstSuccess => () => FirstResult(State.Success),
                ParallelPolicy.FirstFailure => () => FirstResult(State.Failure),
                _ => throw new NotImplementedException(),
            };
        }

        protected override State UpdateNode() => updateMethod();

        private State FirstStop()
        {
            State finalResult = State.Success;

            // Parallel Execution
            foreach (BehaviourTreeNode node in executionList)
            {
                finalResult = node.Update();

                if (finalResult is State.Success or State.Failure)
                {
                    foreach (BehaviourTreeNode child in executionList)
                    {
                        child.Interrupt();
                    }
                    return finalResult;
                }
            }

            return finalResult;
        }

        private State FirstResult(State stopState)
        {
            foreach (BehaviourTreeNode node in executionList.ToList())
            {
                State result = node.Update();

                if (result == stopState)
                {
                    foreach (BehaviourTreeNode child in executionList)
                    {
                        child.Interrupt();
                    }

                    return State.Success;
                }

                if (result != State.Running)
                {
                    executionList.Remove(node);
                }
            }

            return executionList.Count > 0 ? State.Running : State.Failure;
        }

        private State AllStop()
        {
            foreach (BehaviourTreeNode node in executionList.ToList())
            {
                State result = node.Update();

                if (result != State.Running)
                {
                    executionList.Remove(node);
                }
            }

            return executionList.Count > 0 ? State.Running : State.Success;
        }
    }
}
