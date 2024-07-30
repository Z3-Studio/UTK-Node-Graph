using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    public abstract class BehaviourTreeNode : Node
    {
        public State State { get; private set; } = State.Ready;

        // Block set
        protected new float NodeActivationTime { get => base.NodeActivationTime; private set => base.NodeActivationTime = value; }
        public new BehaviourTreeController GraphController => (BehaviourTreeController)base.GraphController;

        public State Update()
        {
            // A little bit faster 
            if (State == State.Running)
            {
                UpdateSimplified();
                return State; // Call Leaf, Decorator or Composite
            }

            State = State.Running;
            NodeActivationTime = Time.time;
            StartNode(); 
            if (State != State.Running)
            {
                StopNode();
                return State;
            }

            UpdateSimplified();
            return State;
        }

        private void UpdateSimplified()
        {
            State = UpdateNode();
            if (State != State.Running)
            {
                StopNode();
            }
        }

        protected virtual void StartNode() { }
        protected abstract State UpdateNode();
        protected virtual void StopNode() { }

        public void Interrupt()
        {
            if (State != State.Running)
                return;

            StopNode();
            State = State.Resting;
            InterruptNode();
        }

        protected virtual void InterruptNode() { }
    }
}
