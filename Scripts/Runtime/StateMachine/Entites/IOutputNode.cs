namespace Z3.NodeGraph.StateMachine
{
    public interface IOutputNode
    {
        bool Startable { get; }
        TransitionList Transitions { get; }
    }
}
