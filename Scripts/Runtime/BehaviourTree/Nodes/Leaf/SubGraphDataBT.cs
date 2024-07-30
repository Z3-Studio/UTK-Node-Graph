using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.SubGraph)]
    public class SubGraphDataBT : LeafNode, ISubGraphNode
    {
        [SerializeField] private GraphData subGraph;

        public GraphData SubGraph => subGraph;
        public GraphController SubController => subController ??= GraphController.BuildSubGraph(subGraph);

        public override string Info => "Sub Graph";

        private GraphController subController;

        public override string SubInfo
        {
            get
            {
                if (!subGraph)
                    return "Empty".AddRichTextColor(Color.gray);

                return $"Run {subGraph.name.ToBold()}";
            }
        }

        protected override void StartNode()
        {
            subController ??= GraphController.BuildSubGraph(subGraph);
            subController.StartGraph();
        }

        protected override State UpdateNode()
        {
            return subController.OnUpdate();
        }

        protected override void StopNode()
        {
            subController.StopGraph();
        }
    }
}
