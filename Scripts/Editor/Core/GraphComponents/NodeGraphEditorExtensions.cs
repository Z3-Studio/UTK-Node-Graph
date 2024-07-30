using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace Z3.NodeGraph.Editor
{
    public static class NodeGraphEditorExtensions
    {
        public static TNodeView GetNodeView<TNodeView>(this Port port) where TNodeView : NodeView
        {
            return GetNodeView<TNodeView>(port.node);
        }

        // TODO: Review it 
        public static TNodeView GetNodeView<TNodeView>(this Node node) where TNodeView : NodeView
        {
            return (node as NGNode).NodeView as TNodeView;
        }

        public static VisualElement CreateNgInspector(this GraphSubAsset obj)
        {
            VisualElement element = new();

            TitleBuilder.AddTitle(element, obj.GetTypeNiceString());

            InspectorElement inspector = new InspectorElement(obj);
            inspector.style.SetPadding(0f);

            HideInGraphBuilder.HideObjects(obj, inspector);

            element.Add(inspector);

            return element;
        }
    }
}