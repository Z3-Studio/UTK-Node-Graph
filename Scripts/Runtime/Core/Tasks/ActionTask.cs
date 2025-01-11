using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    public abstract class ActionTask<T> : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<T> data;

        protected T Agent => data.Value;
    }

    public abstract class ActionTask : Task
    {
        public State State { get; protected set; } = State.Ready;

        // Block set
        protected float NodeActivationTime { get; private set; }
        public float NodeRunningTime => Time.time - NodeActivationTime;

        // TODO: Review it
        // If a event call this method, before the UpdateActionTask, it will reset and call Start again
        private bool forceStop;

        public State UpdateActionTask()
        {
            if (forceStop)
            {
                forceStop = false;
                StopAction();
                return State;
            }

            // Update if is running
            if (State == State.Running)
            {
                UpdateAction();
                if (State != State.Running)
                {
                    forceStop = false;
                    StopAction();
                }
                return State;
            }

            // Start
            State = State.Running;
            NodeActivationTime = Time.time;
            StartAction();

            // Check state
            if (State != State.Running)
            {
                forceStop = false;
                StopAction();
                return State;
            }

            // Update
            if (State == State.Running)
            {
                UpdateAction();
                if (State != State.Running)
                {
                    forceStop = false;
                    StopAction();
                }
                return State;
            }
            return State;
        }

        public void StopTask()
        {
            if (State != State.Running)
                return;

            forceStop = false;
            StopAction();
            State = State.Resting;
        }

        protected void EndAction() => EndAction(true);

        protected void EndAction(bool success)
        {
            forceStop = true;            
            State = success ? State.Success : State.Failure;
        }

        protected virtual void StartAction() { }

        /// <summary> It will be called after running, if you don't call EndAction  </summary>
        protected virtual void UpdateAction() { }
        protected virtual void StopAction() { }
    }
}
