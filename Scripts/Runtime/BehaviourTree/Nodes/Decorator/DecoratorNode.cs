using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [ClassStyle("decorator")]
    public abstract class DecoratorNode : BehaviourTreeNode
    {
        [HideInGraphInspector, ReadOnly]
        public BehaviourTreeNode child;

        protected sealed override void InterruptNode()
        {
            child.Interrupt();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            if (child == null)
                return;

            child = subAssets[child.Guid] as BehaviourTreeNode;
        }

        public override void Parse(Dictionary<string, GraphSubAsset> copies)
        {
            if (child == null)
                return;

            child = copies.GetValueOrDefault(child.Guid) as BehaviourTreeNode;
        }
    }
}
