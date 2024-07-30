using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils.Editor;
using Capacity = UnityEditor.Experimental.GraphView.Port.Capacity;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Wrapper
    /// </summary>
    public sealed class NGNode : Node, INodeGraphElement
    {
        public NodeView NodeView { get; }

        public GraphElement Self => this;
        public NodeGraphReferences References => NodeView.References;

        public NGNode(NodeView nodeView, VisualTreeAsset nodeViewTree) : base(nodeViewTree.GetAssetPath())
        {
            NodeView = nodeView;
            viewDataKey = nodeView.Node.Guid; // kind of metadata store and retrieve the node view information
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeView.OnSetPosition(newPos);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            NodeView.OnSelected();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            NodeView.OnUnselected();
        }

        public override Port InstantiatePort(Orientation orientation, Direction direction, Capacity capacity, Type type)
        {
            return NodeView.InstantiatePort(orientation, direction, capacity, type);
        }

        public void DeleteElement()
        {
            References.Module.DeleteAsset(NodeView.Node);
        }

        public VisualElement GetInspector() => NodeView.GetInspector();
    }
}