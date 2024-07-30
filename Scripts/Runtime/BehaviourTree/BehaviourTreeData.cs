using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [CreateAssetMenu(menuName = GraphPath.Graph + "Behaviour Tree", fileName = "New" + nameof(BehaviourTreeData))]
    public class BehaviourTreeData : GraphData
    {
        public override GraphController CreateInstance(IGraphRunner runner)
        {
            return new BehaviourTreeController(runner, this);
        }

        public void AddConnection(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            if (parent is DecoratorNode decorator)
            {
                decorator.child = child;
            }
            else if (parent is CompositeNode composite)
            {
                composite.children.Add(child);
            }
        }

        public void RemoveConnection(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            if (parent is DecoratorNode decorator)
            {
                decorator.child = null;
            }
            else if (parent is CompositeNode composite)
            {
                composite.children.Remove(child);
            }
        }

        public List<BehaviourTreeNode> GetConnections(BehaviourTreeNode parent)
        {
            if (parent is CompositeNode composite)
            {
                return new List<BehaviourTreeNode>(composite.children);
            }
            else if (parent is DecoratorNode decorator && decorator.child)
            {
                return new List<BehaviourTreeNode>() { decorator.child };
            }

            return new List<BehaviourTreeNode>();
        }
    }
}
