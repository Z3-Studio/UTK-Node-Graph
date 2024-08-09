using System.Linq;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    [CreateAssetMenu(menuName = GraphPath.Graph + "State Machine", fileName = "New" + nameof(StateMachineData))]
    public class StateMachineData : GraphData
    {
        public override GraphController CreateInstance(IGraphRunner runner)
        {
            return new StateMachineController(runner, this);
        }

        public override void SetStartNode(Node node)
        {
            if (node is StateMachineNode fsmNode)
            {
                base.SetStartNode(fsmNode);
            }
        }

        public override void AddSubAsset(GraphSubAsset newNode)
        {
            subAssets.Add(newNode);

            if (startNode == null && newNode is TransitableStateNode smNode and IOutputNode outputNode && outputNode.Startable)
            {
                startNode = smNode;
            }
        }

        public override void RemoveSubAsset(GraphSubAsset node)
        {
            subAssets.Remove(node);

            if (node is TransitableStateNode smNode && smNode == startNode)
            {
                startNode = SubAssets.FirstOrDefault(x => x is Node) as Node;
            }
        }

        public void AddTransition(StateMachineNode nodeParent, Transition newTransition)
        {
            nodeParent.GetTransitions().Add(newTransition);

            subAssets.Add(newTransition);
        }

        public override Node GetAnyStartableNode()
        {
            return subAssets.FirstOrDefault(asset => asset is TransitableStateNode smNode and IOutputNode outputNode && outputNode.Startable) as Node;
        }
    }
}