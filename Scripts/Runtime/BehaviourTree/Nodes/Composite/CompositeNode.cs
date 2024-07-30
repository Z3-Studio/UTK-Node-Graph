using System.Collections;
using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    public abstract class CompositeNode : BehaviourTreeNode, ISubAssetList
    {
        [ReadOnly, HideInGraphInspector]
        public List<BehaviourTreeNode> children = new List<BehaviourTreeNode>();

        public override string ClassStyle => "composite";

        public IList SubAssets => children;

        protected sealed override void SetupDependencies(Dictionary<string, GraphSubAsset> instanceDict)
        {
            List<BehaviourTreeNode> newList = new List<BehaviourTreeNode>();
            foreach (BehaviourTreeNode child in children)
            {
                newList.Add(instanceDict[child.Guid] as BehaviourTreeNode);
            }

            children = newList;
        }

        protected sealed override void InterruptNode()
        {
            foreach (BehaviourTreeNode child in children)
            {
                child.Interrupt();
            }
        }
    }
}
