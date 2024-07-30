namespace Z3.NodeGraph.Core
{
    public interface ISubGraphNode
    {
        public GraphData SubGraph { get; }
        public GraphController SubController { get; }
    }
}
