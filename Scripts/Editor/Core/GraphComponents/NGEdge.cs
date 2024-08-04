using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Line that connects the Ports
    /// </summary>
    public class NGEdge : Edge, INodeGraphElement
    {
        // CreateEdgeControl: Preview or drop a new edge
        // OnCustomStyleResolved: Hover or select
        public NodeGraphReferences References { get; private set; }
        public GraphElement Self => this;

        public NGEdge() : base() { }

        public void InjectDependencies(NodeGraphReferences references)
        {
            References = references;
        }

        public virtual void DeleteElement() { }

        public virtual VisualElement GetInspector() => null;
    }

    public abstract class NGEdge<TEdgeControl> : NGEdge where TEdgeControl : EdgeControl, new()
    {
        // CreateEdgeControl: Preview or drop a new edge
        // OnCustomStyleResolved: Hover or select
        protected TEdgeControl NgEdgeControl => edgeControl as TEdgeControl;

        public NGEdge() : base() { }

        protected override EdgeControl CreateEdgeControl() => new TEdgeControl();
    }
}