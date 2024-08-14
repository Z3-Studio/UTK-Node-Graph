using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for event")]
    public class WaitUntilMediatorEvent : ActionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;

        public override string Info => $"Wait until [{eventMediator}]";

        protected override void StartAction()
        {
            eventMediator.Value.RegisterCallback(EndAction);
        }

        protected override void StopAction()
        {
            eventMediator.Value.UnregisterCallback(EndAction);
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for event")]
    public class WaitUntilMediatorEvent<T> : ActionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;
        [SerializeField] private Parameter<T> value;

        public override string Info => $"Wait until [{eventMediator}] with {value}";

        protected override void StartAction()
        {
            eventMediator.Value.RegisterCallback<T>(OnEventReceived);
        }

        protected override void StopAction()
        {
            eventMediator.Value.UnregisterCallback<T>(OnEventReceived);
        }

        private void OnEventReceived(T receivedValue)
        {
            value.Value = receivedValue;
            EndAction();
        }
    }
}