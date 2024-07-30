using UnityEditor;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    [CustomPropertyDrawer(typeof(Variable), true)]
    public class VariablePropertyDrawer : Z3PropertyDrawer<Variable>
    {
        protected override VisualElement CreateVisualElement()
        {
            return new VariableView(SerializedProperty);
        }
    }
}