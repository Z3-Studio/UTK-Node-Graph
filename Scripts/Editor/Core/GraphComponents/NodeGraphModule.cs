using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Z3.NodeGraph.Core;
using Node = Z3.NodeGraph.Core.Node;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace Z3.NodeGraph.Editor
{
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

            Controller.serializeGraphElements += CutCopyOperation;
            Controller.canPasteSerializedData += AllowPaste;
            Controller.unserializeAndPaste += OnPaste;
        }

        public abstract void OnInitialize();

        public void DeleteElements(params GraphElement[] element)
        {
            UndoRecorder.AddUndo(CurrentGraph, "Delete item");
            Controller.DeleteElements(element);
            AssetDatabase.SaveAssetIfDirty(CurrentGraph);
        }

        public void Dispose()
        {
            Controller.graphViewChanged -= OnGraphViewChanged;

            Controller.serializeGraphElements -= CutCopyOperation;
            Controller.canPasteSerializedData -= AllowPaste;
            Controller.unserializeAndPaste -= OnPaste;

            Controller.DeleteElements(Controller.graphElements);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
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

        #region Copy, Cut, Paste
        private bool AllowPaste(string data) => !Application.isPlaying;

        private string CutCopyOperation(IEnumerable<GraphElement> elements)
        {
            List<string> guid = new();
            foreach (GraphElement itemToCopy in elements)
            {
                guid.Add(itemToCopy.viewDataKey);
            }

            return Serializer.ToJson(guid);
        }

        private void OnPaste(string operationName, string data)
        {
            UndoRecorder.AddUndo(CurrentGraph, "Copy Assets");

            // 1.Find all nodes to be copied
            List<string> objects = Serializer.FromJson<List<string>>(data);

            List<GraphSubAsset> nodesToCopy = objects.Select(guid => Controller.GetNodeByGuid(guid))
                .OfType<NGNode>()
                .Select(node => (GraphSubAsset)node.NodeView.Node)
                .ToList();

            // 2.Find all node dependencies
            List<GraphSubAsset> assetsToCopy = NodeGraphEditorUtils.CollectAllDependencies(nodesToCopy);

            // 3.Create new guid for depedencies
            Dictionary<string, string> guidAssets = new();
            Dictionary<string, GraphSubAsset> clones = new();

            foreach (GraphSubAsset originalAsset in assetsToCopy)
            {
                guidAssets[originalAsset.Guid] = GUID.Generate().ToString();
                clones[originalAsset.Guid] = originalAsset.CloneT(); // Object.Instantiate(node);
            }

            // 4. Create new assets using the new guids, and setup depedencies
            // Offset = Paste Position - Copy Position
            Vector2 offset = Controller.PasteLocalMousePosition - Controller.CopyLocalMousePosition;

            // Key: Original (Copy), Value = Clone (Paste)
            foreach (GraphSubAsset cloneAsset in clones.Values)
            {
                // Update position
                if (cloneAsset is Node cloneNode)
                {
                    cloneNode.Position += offset;
                }

                // Setup name and guid
                string newAssetGuid = guidAssets[cloneAsset.Guid];
                string newName = NodeGraphEditorUtils.ReplaceGuids(cloneAsset.name, guidAssets);

                int parentLastIndex = newName.LastIndexOf('/');
                string newParentName = newName.Substring(0, parentLastIndex + 1);

                cloneAsset.SetGuid(newAssetGuid, newParentName);
                cloneAsset.Parse(clones);

                CurrentGraph.AddSubAsset(cloneAsset);
                AssetDatabase.AddObjectToAsset(cloneAsset, CurrentGraph);

                UndoRecorder.AddCreation(cloneAsset, "Copy Assets");
            }

            AssetDatabase.SaveAssets();
            References.Refresh();
        }
        #endregion
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