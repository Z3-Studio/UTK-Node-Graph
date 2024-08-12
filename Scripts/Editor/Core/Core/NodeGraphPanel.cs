using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.UIBuilder.Editor;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;
using Node = Z3.NodeGraph.Core.Node;

namespace Z3.NodeGraph.Editor
{
    public class GraphClipboard
    {
        public GraphData GraphData { get; }
        public List<GraphSubAsset> Copies { get; } = new List<GraphSubAsset>();
        public Vector2 MousePosition { get; set; }

        public GraphClipboard(GraphData graphData, List<GraphSubAsset> guidList)
        {
            GraphData = graphData;
            Copies = guidList;
        }
    }

    /// <summary>
    /// Simplifies GraphView to create more efficient callbacks to simplify implementation of the new NodeGraphModule
    /// </summary>
    public sealed class NodeGraphPanel : GraphPanel, IDisposable
    {
        public new class UxmlFactory : UxmlFactory<NodeGraphPanel, UxmlTraits> { }

        private NodeGraphModule Module => references.Module;
        private GraphData CurrentGraph => references.Data;

        private NodeGraphReferences references;

        private static GraphClipboard clipboard;

        // Block Commands
        protected override bool canCutSelection => false;
        protected override bool canDuplicateSelection => false;

        internal void Init(NodeGraphReferences nodeGraphReferences)
        {
            references = nodeGraphReferences;
            references.OnChangeGraph += BuildView;
            viewTransformChanged += OnViewChange;

            serializeGraphElements += CutCopyOperation;
            canPasteSerializedData += CanPaste;
            unserializeAndPaste += OnPaste;
        }

        public void Dispose()
        {
            serializeGraphElements -= CutCopyOperation;
            canPasteSerializedData -= CanPaste;
            unserializeAndPaste -= OnPaste;
        }

        internal void ForceRedraw()
        {
            BuildView(CurrentGraph);
        }

        /// <summary> Initialize new GraphData </summary>
        private void BuildView(GraphData graphData)
        {
            Undo.undoRedoPerformed -= OnUndoRedo;

            // TODO: Display messagee when nothing is setted
            if (graphData == null)
            {
                visible = false;
                return;
            }

            visible = true;
            Undo.undoRedoPerformed += OnUndoRedo;

            viewTransform.position = references.Preferences.Position;
            viewTransform.scale = references.Preferences.Scale.ToVector3();
        }

        private void OnViewChange(GraphView graphView)
        {
            if (references.Preferences == null)
                return;

            references.Preferences.Position = graphView.viewTransform.position;
            references.Preferences.Scale = graphView.viewTransform.scale.x;

            UserPreferences.SetInstanceDirty();
        }

        private void OnUndoRedo()
        {
            BuildView(CurrentGraph);
            AssetDatabase.SaveAssets();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return Module.GetCompatiblePorts(startPort, nodeAdapter);
        }

        public Vector2 GetMousePosition(Vector2 viewportPosition)
        {
            Vector2 worldMousePosition = viewportPosition - (Vector2)contentViewContainer.transform.position;
            return worldMousePosition / contentViewContainer.transform.scale.x;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Edge
            if (evt.target is NGEdge edge)
            {
                evt.menu.AppendAction($"Disconnect", action =>
                {
                    DeleteElements(edge);
                    AssetDatabase.SaveAssetIfDirty(CurrentGraph);
                });
                return;
            }

            // Copy, Paste, Delete
            evt.menu.AppendAction("Copy", delegate
            {
                CopySelectionCallback();
            }, (DropdownMenuAction a) => canCopySelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendAction("Paste", delegate
            {
                PasteCallback();
            }, (DropdownMenuAction a) => canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendAction("Delete", delegate
            {
                DeleteSelectionCallback(AskUser.AskUser);
            }, (DropdownMenuAction a) => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendSeparator();

            if (evt.target is NGNode node)
            {
                // Selecting node
                Module.BuildNodeMenu(evt, node.NodeView);
            }
            else if (evt.target is NodeGraphPanel graphView) // this
            {
                // Selection Graph
                Module.BuildGraphMenu(evt);
            }
        }

        public void DeleteElements(params GraphElement[] element) => DeleteElements(element);

        #region Commands
        private string CutCopyOperation(IEnumerable<GraphElement> elements)
        {
            // 1.Find all nodes to be copied
            List<Node> nodesToCopy = elements.Select(itemToCopy => GetNodeByGuid(itemToCopy.viewDataKey))
                .OfType<NGNode>()
                .Select(node => node.NodeView.Node)
                .ToList();

            // 2.Find all node dependencies and create copies
            List<GraphSubAsset> allDependencies = NodeGraphEditorUtils.CollectAllDependencies(nodesToCopy);
                //.Where(a => a is not Node node || nodesToCopy.Contains(node))
                //.ToList();

            // 3. Remove invalid dependencies. Example Transitions without connection
            List<string> itemsToCopy = allDependencies.Select(a => a.Guid).ToList();

            foreach (GraphSubAsset subAsset in allDependencies)
            {
                subAsset.ValidatePaste(itemsToCopy);
            }

            // 4. Create copies to be saved in clipboard
            List<GraphSubAsset> copies = allDependencies.Where(a => itemsToCopy.Contains(a.Guid))
                .Select(a => a.CloneT())
                .ToList();

            clipboard = new GraphClipboard(CurrentGraph, copies);
            return string.Empty;
        }

        private bool CanPaste(string data)
        {
            // Check if GraphData are compatible
            if (!Application.isPlaying && clipboard?.GraphData && CurrentGraph.GetType().IsAssignableFrom(clipboard.GraphData.GetType()))
            {
                // Node: Is not possible to capture mouse position in OnPaste as mousePosition will be in MenuContext
                Vector2 mouseViewport = this.WorldToLocal(Event.current.mousePosition);
                clipboard.MousePosition = GetMousePosition(mouseViewport);
                return true;
            }

            return false;
        }

        private void OnPaste(string operationName, string data)
        {
            UndoRecorder.AddUndo(CurrentGraph, "Copy Nodes and Dependencies");

            // 1.Create new guids
            Dictionary<string, string> guidAssets = new();
            Dictionary<string, GraphSubAsset> clones = new();

            foreach (GraphSubAsset originalAsset in clipboard.Copies)
            {
                // Note: Although the clipboard is already cloned, it is necessary to clone again to avoid having repeated items in case of multiple pastes.
                guidAssets[originalAsset.Guid] = GUID.Generate().ToString();
                clones[originalAsset.Guid] = originalAsset.CloneT();
            }

            // 2. Calculate node offsets
            Vector2 center = clipboard.Copies.OfType<Node>()
                .Select(n => n.Position)
                .CalculateCentroid();

            Vector2 offset = clipboard.MousePosition - center;

            // 3. Create new assets using the new guids, and setup depedencies
            // Key: Original (Copy), Vlue = Clone (Paste)
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
                cloneAsset.Paste(clones);

                CurrentGraph.AddSubAsset(cloneAsset);
                AssetDatabase.AddObjectToAsset(cloneAsset, CurrentGraph);

                UndoRecorder.AddCreation(cloneAsset, "Copy Nodes and Dependencies");
            }

            AssetDatabase.SaveAssets();
            references.Refresh();
        }
        #endregion
    }
}