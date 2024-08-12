using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [ClassStyle("composite")]
    public abstract class CompositeNode : BehaviourTreeNode
    {
        [ReadOnly, HideInGraphInspector]
        public List<BehaviourTreeNode> children = new List<BehaviourTreeNode>();

        protected sealed override void InterruptNode()
        {
            foreach (BehaviourTreeNode child in children)
            {
                child.Interrupt();
            }
        }

        protected sealed override void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            children.ReplaceDependencies(subAssets);
        }

        public sealed override void Paste(Dictionary<string, GraphSubAsset> copies)
        {
            children.Parse(copies);
        }
    }
}
