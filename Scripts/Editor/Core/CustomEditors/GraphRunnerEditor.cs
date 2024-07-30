using UnityEditor;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    [CustomEditor(typeof(GraphRunner), true)]
    public class GraphRunnerEditor : Z3Editor<GraphRunner>
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = base.CreateInspectorGUI();

            Button button = new Button();
            button.text += "Open Node Graph";
            button.clicked += NodeGraphWindow.OpenWindow;
            container.Add(button);

            return container;
        }
    }
}