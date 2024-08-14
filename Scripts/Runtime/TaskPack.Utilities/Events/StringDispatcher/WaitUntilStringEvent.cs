using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class WaitUntilStringEvent : ActionTask
    {
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Wait until [{eventName}]";

        protected override void StartAction()
        {
            StringEvents.RegisterCallback(OnStringEvent);
        }

        protected override void StopAction()
        {
            StringEvents.UnregisterCallback(OnStringEvent);
        }

        private void OnStringEvent(IStringEvent evt)
        {
            if (evt.EventName == eventName.Value)
            {
                EndAction();
            }
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class WaitUntilStringEvent<T> : ActionTask
    {
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> outValue;

        public override string Info => $"Wait until {outValue.GenericType.Name.ToBold()} [{eventName}]";

        protected override void StartAction()
        {
            StringEvents.RegisterCallback<T>(OnStringEvent);
        }

        protected override void StopAction()
        {
            StringEvents.UnregisterCallback<T>(OnStringEvent);
        }

        private void OnStringEvent(IStringEvent<T> evt)
        {
            if (evt.EventName == eventName.Value)
            {
                outValue.Value = evt.Payload;
                EndAction();
            }
        }
    }
}