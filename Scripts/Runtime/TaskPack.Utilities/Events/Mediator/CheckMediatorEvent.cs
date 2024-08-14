using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeDescription("Useful for SM Transitions")]
    [NodeCategory(Categories.Events)]
    public class CheckMediatorEvent : EventConditionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;

        public override string Info => $"{base.Info}: {eventMediator}";

        protected override void Subscribe()
        {
            eventMediator.Value.RegisterCallback(EndEventCondition);
        }

        protected override void Unsubscribe()
        {
            eventMediator.Value.UnregisterCallback(EndEventCondition);
        }
    }

    [NodeDescription("Useful for SM Transitions")]
    [NodeCategory(Categories.Events)]
    public class CheckMediatorEvent<T> : EventConditionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;
        [SerializeField] private Parameter<T> value;

        public override string Info => $"{base.Info}: {eventMediator} with {value}";

        protected override void Subscribe()
        {
            eventMediator.Value.RegisterCallback<T>(OnEventReceived);
        }

        protected override void Unsubscribe()
        {
            eventMediator.Value.UnregisterCallback<T>(OnEventReceived);
        }

        private void OnEventReceived(T receivedValue)
        {
            value.Value = receivedValue;
            EndEventCondition();
        }
    }
}