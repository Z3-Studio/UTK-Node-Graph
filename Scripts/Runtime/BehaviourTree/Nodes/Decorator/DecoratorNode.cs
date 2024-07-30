using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    public abstract class DecoratorNode : BehaviourTreeNode
    {
        [HideInGraphInspector, ReadOnly]
        public BehaviourTreeNode child;

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            if (child == null)
                return;

            child = subAssets[child.Guid] as BehaviourTreeNode;
        }

        public override string ClassStyle => "decorator";

        protected sealed override void InterruptNode()
        {
            child.Interrupt();
        }
    }
}
