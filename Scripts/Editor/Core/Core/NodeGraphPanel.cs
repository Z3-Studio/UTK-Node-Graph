using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.UIBuilder.Editor;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;
using Node = UnityEditor.Experimental.GraphView.Node;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public class NgClipboard
    {
        public GraphData GraphData { get; set; }
        public List<Core.Node> Nodes { get; set; }
        public Vector2 PastePosition { get; set; }
    }

    public class NodeGraphPanel : GraphPanel
    {
        public new class UxmlFactory : UxmlFactory<NodeGraphPanel, UxmlTraits> { }

        private NodeGraphModule Module => references.Module;
        private GraphData CurrentGraph => references.Data;

        private NodeGraphReferences references;

        // TEMP
        public Vector2 CopyLocalMousePosition { get; private set; }
        public Vector2 PasteLocalMousePosition { get; private set; }

        internal void Init(NodeGraphReferences nodeGraphReferences)
        {
            references = nodeGraphReferences;
            references.OnChangeGraph += BuildView;
            viewTransformChanged += OnViewChange;
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
            // TODO REFACTORY
            Vector2 lastMousePosition = evt.localMousePosition;

            if (evt.target is GraphView || evt.target is Node || evt.target is Group)
            {
                evt.menu.AppendAction("Copy", delegate
                {
                    CopyLocalMousePosition = GetMousePosition(lastMousePosition);
                    CopySelectionCallback();
                }, (DropdownMenuAction a) => canCopySelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Paste", delegate
                {
                    PasteLocalMousePosition = GetMousePosition(lastMousePosition);
                    PasteCallback();
                }, (DropdownMenuAction a) => canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            // https://www.youtube.com/watch?v=F4cTWOxMjMY&t=158s
            //SearchWindowContext ctx = new();

            //VisualElement windowRoot = references.Window.rootVisualElement;
            //Vector2 worldMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, ctx.screenMousePosition - references.Window.position.position);
            //Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            //UnityEngine.Debug.Log(localMousePosition);

            // Review it: Missing actions like "Disconnect All"

            if (evt.target is NGNode node)
            {
                base.BuildContextualMenu(evt); // Cut, Copy, Paste, Delete, Duplicate
                Module.BuildNodeMenu(evt, node.NodeView);
            }
            else if (evt.target is NodeGraphPanel graphView) // this
            {
                Module.BuildGraphMenu(evt);
            }
        }
    }

    public static class UndoRecorder // TODO: Improve and clean
    {
        public static void AddUndo(Object context, string action) // Make patterns
        {
            Undo.RecordObject(context, $"NodeGraph: {action} {context.GetType().Name}");
        }

        public static void AddCreation(Object context, string action) // Make patterns
        {
            Undo.RegisterCreatedObjectUndo(context, $"NodeGraph: {action} {context.GetType().Name}");
        }
    }
}