using System;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    public abstract class ActionTask<T> : ActionTask where T : class
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<T> data;

        public string AgentInfo => data.ToString();
        public T Agent => data.Value;
    }

    public abstract class ActionTask : Task
    {
        public State State { get; protected set; } = State.Ready;

        // Block set
        protected float NodeActivationTime { get; private set; }
        public float NodeRunningTime => Time.time - NodeActivationTime;

        public State UpdateActionTask()
        {
            // Update if is running
            if (State == State.Running)
            {
                UpdateAction();
                if (State != State.Running)
                {
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
                StopAction();
                return State;
            }

            // Update
            if (State == State.Running)
            {
                UpdateAction();
                if (State != State.Running)
                {
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

            StopAction();
            State = State.Resting;
        }

        protected void EndAction(bool successful = true)
        {
            State = successful ? State.Success : State.Failure;
        }

        protected virtual void StartAction() { }

        /// <summary> It will be called after running, if you don't call EndAction  </summary>
        protected virtual void UpdateAction() { }
        protected virtual void StopAction() { }
    }
}
