using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all Dispatcher References")]
    public class SendStringEventToAllReferences : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<StringDispatcherReferences> dispatcherReferences;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Send Event [{eventName}] to all references key";

        protected override void StartAction()
        {
            dispatcherReferences.Value.SendEventAll(GraphRunner, eventName.Value);
            EndAction();
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all Dispatcher References with payload")]
    public class SendStringEventToAllReferences<T> : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<StringDispatcherReferences> dispatcherReferences;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> valueToSend;

        public override string Info => $"Send Event [{eventName}] to all references key with {valueToSend}";

        protected override void StartAction()
        {
            dispatcherReferences.Value.SendEventAll(GraphRunner, eventName.Value, valueToSend.Value);
            EndAction();
        }
    }
}