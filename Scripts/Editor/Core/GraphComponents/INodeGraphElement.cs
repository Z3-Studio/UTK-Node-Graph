using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Used for calling extra methods on objects of type <see cref="GraphElement"/>
    /// </summary>
    public interface INodeGraphElement
    {
        GraphElement Self { get; }
        NodeGraphReferences References { get; } // Maybe module is better?
        void DeleteElement();
        VisualElement GetInspector();
    }
}