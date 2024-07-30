using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.BehaviourTree;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Capacity = UnityEditor.Experimental.GraphView.Port.Capacity;
using Node = Z3.NodeGraph.Core.Node;

namespace Z3.NodeGraph.Editor
{
    public class BTNodeView : NodeView
    {
        [UIElement] private VisualElement nodeState;

        public Color ColorState => nodeState.resolvedStyle.backgroundColor;

        public BTPort Input { get; }
        public BTPort Output { get; }

        public new BehaviourTreeNode Node => (BehaviourTreeNode)base.Node;
        protected new BehaviourTreeGraphModule GraphModule => base.GraphModule as BehaviourTreeGraphModule;
        protected new BehaviourTreeData GraphData => base.GraphData as BehaviourTreeData;

        private State lastState = State.Ready;
        private float delayColor;
        private float oldStart;

        private const float Delay = .7f;

        #region Initialization
        public BTNodeView(NodeGraphReferences references, BehaviourTreeNode node) : base(references, node)
        {
            // Setup Ports
            Input = Node switch
            {
                BehaviourTreeNode => InstantiatePort(Direction.Input, Capacity.Single),
                _ => null
            };

            Output = Node switch
            {
                DecoratorNode => InstantiatePort(Direction.Output, Capacity.Single),
                CompositeNode => InstantiatePort(Direction.Output, Capacity.Multi),
                _ => null
            };

            if (Input != null)
            {
                InputContainer.Add(Input);
            }

            if (Output != null)
            {
                OutputContainer.Add(Output);
            }
        }

        public override VisualTreeAsset GetViewTree() => NodeGraphResources.BTNodeVT;

        public override void OnNodesInitialized()
        {
            foreach (BehaviourTreeNode child in GraphData.GetConnections(Node))
            {
                BTNodeView childView = GraphModule.FindNodeView<BTNodeView>(child);
                Output.InstantiateEdge<BTEdge>(childView.Input, References);
            }
        }

        /// <summary> Simplified and converted version </summary>
        protected BTPort InstantiatePort(Direction direction, Capacity capacity)
        {
            return new BTPort(Orientation.Vertical, direction, capacity, typeof(bool));
        }

        public override NGPort InstantiatePort(Orientation orientation, Direction direction, Capacity capacity, Type type)
        {
            return InstantiatePort(direction, capacity);
        }
        #endregion

        public void SortChildren()
        {
            if (Node is CompositeNode composite)
            {
                // Sort By Horizontal Position
                composite.children.Sort((BehaviourTreeNode left, BehaviourTreeNode right) =>
                {
                    return left.Position.x < right.Position.x ? -1 : 1;
                });
            }
        }

        protected override void OnUpdateUI()
        {
            base.OnUpdateUI();

            if (!Application.isPlaying)
                return;

            State newState = Node.State;
            if (newState != lastState || Node.NodeActivationTime != oldStart)
            {
                oldStart = Node.NodeActivationTime;

                RemoveFromClassList(lastState.ToString().ToLower());
                AddToClassList(newState.ToString().ToLower());
                delayColor = Time.time + Delay;
            }
            else if (newState is State.Success or State.Failure)
            {

                if (Time.time > delayColor)
                {
                    RemoveFromClassList(newState.ToString().ToLower());
                }
            }

            lastState = newState;

            //description.text = newState.ToString();
        }
    }
}