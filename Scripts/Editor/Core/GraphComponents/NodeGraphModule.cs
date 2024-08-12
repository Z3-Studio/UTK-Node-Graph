using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.Core;
using Node = Z3.NodeGraph.Core.Node;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Abstraction for creating new GraphData editors
    /// </summary>
    public abstract class NodeGraphModule
    {
        protected GraphData CurrentGraph => References.Data;
        protected NodeGraphReferences References { get;  set; }
        protected NodeGraphPanel Controller => References.Graph;
        public UQueryState<UnityEditor.Experimental.GraphView.Node> nodes => Controller.nodes;
        public UQueryState<Port> ports => Controller.ports;
        public UQueryState<Edge> edges => Controller.edges;

        private NodeView startNode;

        public virtual void Construct(NodeGraphReferences references)
        {
            References = references;
            Controller.graphViewChanged += OnGraphViewChanged;
        }

        public abstract void OnInitialize();

        public void Dispose()
        {
            Controller.graphViewChanged -= OnGraphViewChanged;

            Controller.DeleteElements(Controller.graphElements);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                UndoRecorder.AddUndo(CurrentGraph, "Delete item");
                OnElementsToRemove(graphViewChange.elementsToRemove);
            }

            if (graphViewChange.edgesToCreate != null)
            {
                List<NGEdge> edgesToCreate = graphViewChange.edgesToCreate.OfType<NGEdge>().ToList();
                edgesToCreate.ForEach(e => e.InjectDependencies(References));
                OnEdgesToCreate(edgesToCreate);
            }

            if (graphViewChange.movedElements != null)
            {
                OnMovedElements(graphViewChange.movedElements);
            }

            // Set dirty?

            return graphViewChange;
        }
        
        protected virtual void OnEdgesToCreate(List<NGEdge> edgesToCreate) { }
        protected virtual void OnMovedElements(List<GraphElement> movedElements) { }
        protected virtual void OnElementsToRemove(List<GraphElement> elementsToRemove)
        {
            foreach (INodeGraphElement graphElement in elementsToRemove.OfType<INodeGraphElement>())
            {
                graphElement.DeleteElement();
            }
        }

        /// <summary> Defines compatibility between ports, in short, Inputs must connect to Outputs </summary>
        public virtual List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();
        }

        public virtual void BuildGraphMenu(ContextualMenuPopulateEvent evt)
        {

        }

        public virtual void BuildNodeMenu(ContextualMenuPopulateEvent evt, NodeView node)
        {
            Action<DropdownMenuAction> action = _ =>
            {
                CurrentGraph.SetStartNode(node.Node);

                startNode.SetStart(false);
                startNode = node;
                startNode.SetStart(true);
            };

            Status status = CurrentGraph.StartNode == node.Node ? Status.Checked : Status.Normal;
            evt.menu.AppendAction("Set as Start Note", action, status);
        }

        protected void CreateNode(Type type, Vector2 mouseViewportPosition)
        {
            // Instantiate and setup
            Node newNode = ScriptableObject.CreateInstance(type) as Node;
            newNode.SetGuid(GUID.Generate().ToString());

            // Set position
            newNode.Position = Controller.GetMousePosition(mouseViewportPosition);

            // Before to add and remember
            UndoRecorder.AddUndo(CurrentGraph, "Create Node");

            // Add and update view
            CurrentGraph.AddSubAsset(newNode);
            CreateNodeView(newNode);

            // Add and remember
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(newNode, CurrentGraph);
            }
            UndoRecorder.AddCreation(newNode, "Create Node");

            AssetDatabase.SaveAssets();
        }

        protected void InitializeNodes<TNode>(List<TNode> nodeList) where TNode : Node
        {
            List<NodeView> nodeViewList = new();

            foreach (TNode node in nodeList)
            {
                NodeView nodeView = CreateNodeView(node);
                nodeViewList.Add(nodeView);
            }

            foreach (NodeView nodeView in nodeViewList)
            {
                nodeView.OnNodesInitialized();
            }
        }

        protected NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = MakeNodeView(node);
            if (CurrentGraph.StartNode == node)
            {
                startNode = nodeView;
                nodeView.SetStart(true);
            }

            Controller.AddElement(nodeView.NGNode);

            return nodeView;
        }

        protected abstract NodeView MakeNodeView(Node node); // TODO: Integrate with ModuleCreator to work by inheritance with best match

        public virtual void Inspect(INodeGraphElement graphElement)
        {
            References.Inspector.Clear();

            if (graphElement == null)
                return;

            VisualElement inspector = graphElement.GetInspector();
            References.Inspector.Add(inspector);
        }

        public TNodeView FindNodeView<TNodeView>(Node node) where TNodeView : NodeView
        {
            NGNode ngNode = Controller.GetNodeByGuid(node.Guid) as NGNode;
            return ngNode.NodeView as TNodeView;
        }

        public void AddElement(GraphElement graphElement)
        {
            Controller.AddElement(graphElement);
        }
    }

    public abstract class NodeGraphModule<TGraphData> : NodeGraphModule where TGraphData : GraphData
    {
        protected new TGraphData CurrentGraph => (TGraphData)base.CurrentGraph;

        public sealed override void Construct(NodeGraphReferences references)
        {
            base.Construct(references);
        }
    }
}