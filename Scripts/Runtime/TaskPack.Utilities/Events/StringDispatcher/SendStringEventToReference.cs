using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher Reference")]
    public class SendStringEventToReference : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<StringDispatcherReferences> dispatcherReferences;
        [SerializeField] private Parameter<string> dispatcherKey;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Send Event [{eventName}] to '{dispatcherKey}' key";

        protected override void StartAction() 
        {
            dispatcherReferences.Value.SendEventTo(dispatcherKey.Value, GraphRunner, eventName.Value);
            EndAction();
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher Reference with payload")]
    public class SendStringEventToReference<T> : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<StringDispatcherReferences> dispatcherReferences;
        [SerializeField] private Parameter<string> dispatcherKey;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> valueToSend;

        public override string Info => $"Send Event [{eventName}] to '{dispatcherKey}' key with {valueToSend}";

        protected override void StartAction()
        {
            dispatcherReferences.Value.SendEventTo(dispatcherKey.Value, GraphRunner, eventName.Value, valueToSend.Value);
            EndAction();
        }
    }
}