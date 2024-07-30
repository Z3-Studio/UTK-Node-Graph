using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils.ExtensionMethods;
using Node = Z3.NodeGraph.Core.Node;
using Capacity = UnityEditor.Experimental.GraphView.Port.Capacity;

namespace Z3.NodeGraph.Editor
{
    public abstract class NodeView
    {
        [UIElement] protected VisualElement start;
        [UIElement] protected VisualElement icon;
        [UIElement] protected Label info;
        [UIElement] protected Label description;

        // Dependencies
        public Node Node { get; }
        public NGNode NGNode { get; }
        public NodeGraphReferences References { get; }

        // References
        protected NodeGraphModule GraphModule => References.Module;
        protected GraphData GraphData => References.Data;
        protected VisualElement InputContainer => NGNode.inputContainer;
        protected VisualElement OutputContainer => NGNode.outputContainer;

        public NodeView(NodeGraphReferences references, Node node)
        {
            References = references;
            Node = node;

            NGNode = new NGNode(this, GetViewTree());
            NGNode.BindUIElements(this);

            SetupStyle();

            NGNode.RegisterUpdate(OnUpdateUI);
            NGNode.RegisterCallback<MouseDownEvent>(OnMouseClick);
        }

        public abstract VisualTreeAsset GetViewTree();
        public virtual VisualElement GetInspector() => Node.CreateNgInspector();

        /// <summary> Called only after initial loading of the graph </summary>
        /// <remarks> This can be used to instantiate items like edges </remarks>
        public virtual void OnNodesInitialized() { }

        /// <summary>
        /// Virtual method to customize <see cref="Port"/> and <see cref="Edge"/> style and functionality
        /// <para> Similar implementation to -> <see cref="UnityEditor.Experimental.GraphView.Node.InstantiatePort(Orientation, Direction, Capacity, Type)"/> </para>
        /// </summary>
        public virtual NGPort InstantiatePort(Orientation orientation, Direction direction, Capacity capacity, Type portType)
        {
            NGPort port = new NGPort(orientation, direction, capacity, portType);
            port.AddPortManipulator<NGEdge>();
            return port;
        }

        public virtual void OnSetPosition(Rect newPos)
        {
            UndoRecorder.AddUndo(Node, "Set position");
            Node.Position = newPos.position; // Same than newPos.min
            EditorUtility.SetDirty(Node);
        }

        public virtual void OnSelected()
        {
            References.InvokeUpdateSelection(NGNode);
        }

        public virtual void OnUnselected()
        {
            References.InvokeUpdateSelection(null);
        }

        protected virtual void OnUpdateUI()
        {
            NGNode.title = Node.ToString();
            string infoText = Node.SubInfo;
            info.text = infoText;
            info.style.SetDisplay(!string.IsNullOrEmpty(infoText));
        }

        [Obsolete("TODO: Move to derived classes")]
        public void SetStart(bool displayStart) => start.visible = displayStart;

        protected void RemoveFromClassList(string classStyle) => NGNode.RemoveFromClassList(classStyle);

        protected void AddToClassList(string classStyle) => NGNode.AddToClassList(classStyle);

        private void OnMouseClick(MouseDownEvent e)
        {
            // Check if is left, double click and Node is a valid sub graph
            if (e.button == 0 && e.clickCount == 2 && Node is ISubGraphNode subGraph)
            {
                References.OpenSubGraph(subGraph);
            }
        }

        private void SetupStyle()
        {
            // Reset preview
            start.visible = false;
            description.text = string.Empty;

            // Set node position
            NGNode.style.left = Node.Position.x;
            NGNode.style.top = Node.Position.y;

            SetupAttributes();

            // Define color
            NGNode.AddToClassList(Node.ClassStyle);
        }

        private void SetupAttributes()
        {
            // Set Icon
            NodeIconAttribute attribute = Node.GetType().GetCustomAttribute<NodeIconAttribute>(true);

            Texture2D iconTexture = Texture2D.whiteTexture;
            if (attribute != null)
            {
                iconTexture = NodeGraphResources.GetIGraphIcon(attribute.IconType);
            }

            icon.style.backgroundImage = iconTexture;
        }
    }
}