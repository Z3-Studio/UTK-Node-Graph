using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher")]
    public class SendStringEvent : ActionTask
    {
        [SerializeField] private Parameter<GameObject> target;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Send Event [{eventName}] to {target}";

        protected override void StartAction()
        {
            StringEventDispatcher.SendEvent(GraphRunner, target.Value, eventName.Value);
            EndAction();
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher with payload")]
    public class SendStringEvent<T> : ActionTask
    {
        [SerializeField] private Parameter<GameObject> target;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> valueToSend;

        public override string Info => $"Send Event [{eventName}] to {target} with {valueToSend}";

        protected override void StartAction()
        {
            StringEventDispatcher.SendEvent(GraphRunner, target.Value, eventName.Value, valueToSend.Value);
            EndAction();
        }
    }
}