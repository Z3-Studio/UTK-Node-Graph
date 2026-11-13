using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Tasks
{
    public abstract class ConditionTask : Task
    {
        [SerializeField] private bool invertCondition;

        public sealed override string Info => invertCondition ? "! ".AddRichTextColor(Color.magenta).ToBold() + InfoC : InfoC;
        public virtual string InfoC => base.Info;

        public bool EvaluateCondition()
        {
            bool conditionMet = CheckCondition();
            return conditionMet != invertCondition;
        }

        public virtual void StartCondition() { } // Note: Using SM with OnlyWhenFinished and Repeat, it start and stop in the same frame
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
