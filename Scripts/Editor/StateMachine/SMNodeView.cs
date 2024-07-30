using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.StateMachine;
using Z3.UIBuilder;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Capacity = UnityEditor.Experimental.GraphView.Port.Capacity;

namespace Z3.NodeGraph.Editor
{
    public class SMNodeView : NodeView
    {
        [UIElement] private VisualElement contents;
        [UIElement] private VisualElement nodeState;

        public Rect Layout => contents.layout;
        public Color ColorState => nodeState.resolvedStyle.backgroundColor;
        public SMPort Output { get; }

        public new StateMachineNode Node => base.Node as StateMachineNode;
        private new StateMachineGraphModule GraphModule => base.GraphModule as StateMachineGraphModule;


        private float delayColor;
        private State lastState = State.Ready;
        private float oldStart = -1;

        public const float Delay = .7f;

        public SMNodeView(NodeGraphReferences references, StateMachineNode node) : base(references, node)
        {
            // Note: AnyState is not TransitableStateNode and have Output
            if (node is IOutputNode) 
            {
                Output = InstantiatePort(Direction.Output);
                OutputContainer.Add(Output);

                return; 
            }

            if (node is not TransitableStateNode)
            {
                AddToClassList("non-transitable");
            }
        }

        public override VisualTreeAsset GetViewTree() => NodeGraphResources.SMNodeVT;

        public override VisualElement GetInspector()
        {
            if (Node is IOutputNode outputNode)
            {
                return CreateTransitableInspector(outputNode.Transitions);
            }
            else
            {
                return Node.CreateNgInspector();
            }
        }

        public override void OnNodesInitialized()
        {
            if (Node is not IOutputNode)
                return;

            List<TransitableStateNode> transitions = Node.GetTransitions()
                .Select(t => t.Connection)  // Get Nodes
                .Distinct()                           // Only once to avoid two edges
                .ToList();

            transitions.ForEach(nodeChild =>
            {
                SMTransitableNodeView child = GraphModule.FindNodeView<SMTransitableNodeView>(nodeChild);
                Output.InstantiateEdge<SMEdge>(child.Input, References);
            });
        }

        /// <summary> Simplified and converted version </summary>
        protected SMPort InstantiatePort(Direction direction)
        {
            return new SMPort(Orientation.Vertical, direction, Capacity.Multi, typeof(bool));
        }

        public override NGPort InstantiatePort(Orientation orientation, Direction direction, Capacity capacity, Type type)
        {
            return InstantiatePort(direction);
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

                lastState = newState;
            }
            else if (newState is State.Success or State.Failure)
            {
                if (Time.time > delayColor)
                {
                    RemoveFromClassList(newState.ToString().ToLower());
                }
            }
        }

        /// <summary> It will create an Inspector with Node and Transitions </summary>
        private VisualElement CreateTransitableInspector(List<Transition> transitions)
        {
            // Tabs
            TabView tabView = new TabView();

            Tab tabExecution = new("Execution", EditorIcons.GetTexture2D(IconType.Play));
            tabExecution.style.marginTop = 10;

            Tab tabTransitions = new("Transitions", EditorIcons.GetTexture2D(IconType.Pivot));
            tabTransitions.style.marginTop = 10;

            tabView.Add(tabExecution);
            tabView.Add(tabTransitions);

            // Execution
            tabExecution.Add(Node.CreateNgInspector());

            // Transitions
            Z3ListViewConfig transitionConfig = new("Transitions")
            {
                showAddBtn = false,
                toStringWithPrefix = false,
            };

            // TODO: Move Transition Check type to inside the Transition Tab
            GraphSubAssetListView<Transition> transitionListElement = new(References, Node, transitions, transitionConfig);
            tabTransitions.Add(transitionListElement);

            VisualElement transitionContainer = new();
            transitionListElement.Add(transitionContainer);

            // TODO: Check if need disconnect when delete
            // TODO: Select node again
            transitionListElement.onDelete += (t) => References.ForceRedraw();

            return tabView;
        }
    }

    /// <summary>
    /// Node with transitions
    /// </summary>
    public class SMTransitableNodeView : SMNodeView
    {
        public SMPort Input { get; }
        public new TransitableStateNode Node => (TransitableStateNode)base.Node;

        public SMTransitableNodeView(NodeGraphReferences references, TransitableStateNode node) : base(references, node)
        {
            Input = InstantiatePort(Direction.Input);
            InputContainer.Add(Input);
        }
    }
}