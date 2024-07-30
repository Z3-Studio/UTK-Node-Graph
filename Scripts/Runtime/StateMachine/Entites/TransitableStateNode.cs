using UnityEngine;

namespace Z3.NodeGraph.StateMachine
{
    /// <summary>
    /// Node with Input
    /// </summary>
    public abstract class TransitableStateNode : StateMachineNode 
    {
        public bool Active { get; private set; }

        public virtual void StartState()
        {
            Active = true;
            NodeActivationTime = Time.time;
        }

        public virtual void UpdateState() { }

        public virtual void StopState() 
        {
            Active = false;
        }
    }
}
