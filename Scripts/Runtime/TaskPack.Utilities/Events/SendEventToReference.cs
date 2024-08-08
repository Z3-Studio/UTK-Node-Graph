using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{ 
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to a graph")]
    public class SendEventToReference : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GraphReferences> data;
        [SerializeField] private Parameter<string> graph;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Send Event to {graph} [{eventName}]";

        protected override void StartAction() 
        {
            GraphRunner owner = data.Value.GetGraph(graph.Value);
            owner.SendEvent(eventName.Value);
            EndAction();
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to a graph passing a value")]
    public class SendEventToReference<T> : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GraphReferences> data;
        [SerializeField] private Parameter<string> graph;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> value;

        public override string Info => $"Send Event to {graph} [{eventName}]\nPassing {value}";

        protected override void StartAction() 
        {
            GraphRunner owner = data.Value.GetGraph(graph.Value);
            owner.SendEvent(eventName.Value, value.Value);
            EndAction();
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all graphs at Graph References")]
    public class SendEventToReferences : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GraphReferences> data;
        [SerializeField] private Parameter<string> eventName;
        
        public override string Info => $"Send {eventName.Value} to all graphs";

        protected override void StartAction() 
        {
            GraphReference[] owners = data.Value.Graphs;
            
            foreach (GraphReference owner in owners)
                owner.graphOwner.SendEvent(eventName.Value);
            
            EndAction();
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all graphs at Graph References")]
    public class SendEventToReferences<T> : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GraphReferences> data;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> value;
        
        public override string Info => $"Send {eventName.Value} to all graphs\nPassing {value}";

        protected override void StartAction() 
        {
            GraphReference[] owners = data.Value.Graphs;
            
            foreach (GraphReference owner in owners)
                owner.graphOwner.SendEvent(eventName.Value, value.Value);
            
            EndAction();
        }
    }
}