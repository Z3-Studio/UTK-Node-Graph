using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    public class BehaviourTreeController : GraphController<BehaviourTreeData>
    {
        private BehaviourTreeNode RootNode => GraphData.StartNode as BehaviourTreeNode;

        public BehaviourTreeController(IGraphRunner runner, BehaviourTreeData data) : base(runner, data) { }

        public override State OnUpdate() => RootNode.Update();
    }
}
