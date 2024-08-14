using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    public abstract class ConditionTask<T> : ConditionTask where T : class
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<T> data;

        public T Agent => data.Value;
        public string AgentInfo => data.ToString();
    }

    public abstract class ConditionTask : Task
    {
        [SerializeField] private bool invertCondition;

        public bool InvertCondition => invertCondition;

        public bool EvaluateCondition()
        {
            bool conditionMet = CheckCondition();
            return invertCondition ? !conditionMet : conditionMet;
        }

        public virtual void StartCondition() { }
        public abstract bool CheckCondition();
        public virtual void StopCondition() { }
    }

    /// <summary>
    /// Event Conditions are more efficient and powerful in StateMachines.
    /// Not recommended for use in BehaviourTrees
    /// </summary>
    public abstract class EventConditionTask : ConditionTask
    {
        private bool actionCalled;

        public sealed override void StartCondition()
        {
            actionCalled = false;
            Subscribe();
        }

        public sealed override void StopCondition()
        {
            Unsubscribe();
        }

        public sealed override bool CheckCondition() => actionCalled;

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected void EndEventCondition() => actionCalled = true;
    }
}
