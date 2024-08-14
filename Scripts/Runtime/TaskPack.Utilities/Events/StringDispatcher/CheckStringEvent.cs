using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeDescription("Useful for SM Transitions")]
    [NodeCategory(Categories.Events)]
    public class CheckStringEvent : EventConditionTask
    {
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"{base.Info}: {eventName}";

        protected override void Subscribe()
        {
            StringEvents.RegisterCallback(OnCustomEvent);
        }

        protected override void Unsubscribe()
        {
            StringEvents.UnregisterCallback(OnCustomEvent);
        }

        private void OnCustomEvent(IStringEvent evt)
        {
            if (evt.EventName == eventName.Value)
            {
                EndEventCondition();
            }
        }
    }

    [NodeDescription("Useful for SM Transitions")]
    [NodeCategory(Categories.Events)]
    public class CheckStringEvent<T> : EventConditionTask
    {
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> returnedValue;

        public override string Info => $"{base.Info}: {eventName}";

        protected override void Subscribe()
        {
            StringEvents.RegisterCallback<T>(OnCustomEvent);
        }

        protected override void Unsubscribe()
        {
            StringEvents.UnregisterCallback<T>(OnCustomEvent);
        }

        private void OnCustomEvent(IStringEvent<T> evt)
        {
            if (evt.EventName == eventName.Value)
            {
                returnedValue = evt.Payload;
                EndEventCondition();
            }
        }
    }
}