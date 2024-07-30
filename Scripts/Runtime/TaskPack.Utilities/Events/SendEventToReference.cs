using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{ 
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to a graph")]
    public class SendEventToReference : ActionTask<GraphReferences> 
    {
        /*[RequiredField]*/ public Parameter<string> graph;
        /*[RequiredField]*/ public Parameter<string> eventName;

        public override string Info => $"Send Event to {graph} [{eventName}]";

        protected override void StartAction() 
        {
            GraphRunner owner = Agent.GetGraph(graph.Value);
            owner.SendEvent(eventName.Value);
            EndAction(true);
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to a graph passing a value")]
    public class SendEventToReference<T> : ActionTask<GraphReferences> 
    {
        /*[RequiredField]*/ public Parameter<string> graph;
        /*[RequiredField]*/ public Parameter<string> eventName;
        public Parameter<T> value;

        public override string Info => $"Send Event to {graph} [{eventName}]\nPassing {value}";

        protected override void StartAction() 
        {
            GraphRunner owner = Agent.GetGraph(graph.Value);
            owner.SendEvent(eventName.Value, value.Value);
            EndAction(true);
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all graphs at Graph References")]
    public class SendEventToReferences : ActionTask<GraphReferences>
    {
        /*[RequiredField]*/ public Parameter<string> eventName;
        
        public override string Info => $"Send {eventName.Value} to all graphs";

        protected override void StartAction() 
        {
            GraphReference[] owners = Agent.Graphs;
            
            foreach (GraphReference owner in owners)
                owner.graphOwner.SendEvent(eventName.Value);
            
            EndAction(true);
        }
    }
    
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to all graphs at Graph References")]
    public class SendEventToReferences<T> : ActionTask<GraphReferences>
    {
        /*[RequiredField]*/ public Parameter<string> eventName;
        public Parameter<T> value;
        
        public override string Info => $"Send {eventName.Value} to all graphs\nPassing {value}";

        protected override void StartAction() 
        {
            GraphReference[] owners = Agent.Graphs;
            
            foreach (GraphReference owner in owners)
                owner.graphOwner.SendEvent(eventName.Value, value.Value);
            
            EndAction(true);
        }
    }
}