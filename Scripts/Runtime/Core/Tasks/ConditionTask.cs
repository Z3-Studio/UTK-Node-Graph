using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    public abstract class ConditionTask<T> : ConditionTask where T : class
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<T> data;

        public string AgentInfo => data.ToString();
        public T Agent => data.Value;
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

    public abstract class EventConditionTask : ConditionTask
    {
        private bool actionCalled;

        public sealed override void StartCondition()
        {
            Subscribe();
        }

        public sealed override void StopCondition()
        {
            Unsubscribe();
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected void EndEventCondition() => actionCalled = true;

        public sealed override bool CheckCondition()
        {
            bool value = actionCalled;
            actionCalled = false;
            return value;
        }
    }
}
