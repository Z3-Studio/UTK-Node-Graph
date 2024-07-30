using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    public abstract class StateMachineNode : Node
    {
        /// <summary> Used to update the visual elements in editor </summary>
        public State State { get; protected set; } = State.Ready;
        public new StateMachineController GraphController => (StateMachineController)base.GraphController;

        public virtual void StartGraph() { }

        public virtual void StopGraph() { }
    }
}
