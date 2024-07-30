using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeDescription("You should not use it condition BT task")]
    public class CheckEvent : EventConditionTask
    {
        public Parameter<string> eventName;

        public override string Info => $"{name}: {eventName}";

        protected override void Subscribe()
        {
            Events.OnCustomEvent += OnCustomEvent;
        }

        protected override void Unsubscribe()
        {
            Events.OnCustomEvent -= OnCustomEvent;
        }

        private void OnCustomEvent(string sentEventName, object value, Component sender)
        {
            if (sentEventName.Equals(eventName.Value, System.StringComparison.OrdinalIgnoreCase))
            {
                EndEventCondition();
            }
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class WaitUntilEvent : ActionTask
    {
        public Parameter<string> eventName;

        public override string Info => $"Wait until [{eventName}]";

        protected override void StartAction()
        {
            Events.OnCustomEvent += OnCustomEvent;
        }

        protected override void StopAction()
        {
            Events.OnCustomEvent -= OnCustomEvent;
        }

        private void OnCustomEvent(string sentEventName, object value, Component sender)
        {
            if (sentEventName.Equals(eventName.Value, System.StringComparison.OrdinalIgnoreCase))
            {
                EndAction(true);
            }
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class WaitUntilEvent<T> : ActionTask
    {
        public Parameter<string> eventName;
        public Parameter<T> outValue;

        //public override string Info => $"Wait until <<b>{outValue.varType.Name}</b>> [{eventName}]";

        protected override void StartAction()
        {
            //router.onCustomEvent += OnCustomEvent;
        }

        protected override void StopAction()
        {
            //router.onCustomEvent -= OnCustomEvent;
        }

        //private void OnCustomEvent(string sentEventName, IEventData data)
        //{
        //    if (sentEventName.Equals(eventName.Value, System.StringComparison.OrdinalIgnoreCase) && data is EventData<T> eventData)
        //    {
        //        outValue.Value = eventData.Value;
        //        EndAction(true);
        //    }
        //}
    }
}