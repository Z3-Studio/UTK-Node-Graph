using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class WaitUntilStringEvent : ActionTask
    {
        [SerializeField] private Parameter<StringEventDispatcher> stringEvent;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Wait Animation Event [{eventName}]";

        protected override void StartAction()
        {
            stringEvent.Value.RegisterCallback(OnEventTrigger);
        }

        protected override void StopAction()
        {
            stringEvent.Value.UnregisterCallback(OnEventTrigger);
        }

        private void OnEventTrigger(IStringEvent sentEventName)
        {
            if (!sentEventName.EventName.Equals(eventName.Value, StringComparison.OrdinalIgnoreCase))
                return;

            EndAction();
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Waits for a graph event")]
    public class OnStringEvent : EventConditionTask
    {
        [SerializeField] private Parameter<StringEventDispatcher> stringEvent;
        [SerializeField] private Parameter<string> eventName;

        public override string InfoC => $"On String Event [{eventName}]";

        protected override void Subscribe()
        {
            stringEvent.Value.RegisterCallback(eventName, EndEventCondition);
        }

        protected override void Unsubscribe()
        {
            stringEvent.Value.UnregisterCallback(eventName, EndEventCondition);
        }
    }
}