using UnityEditor;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    [CustomPropertyDrawer(typeof(Parameter<>), true)]
    public class ParameterPropertyDrawer : Z3PropertyDrawer<IParameter>
    {
        protected override VisualElement CreateVisualElement()
        {
            return new ParameterView(SerializedProperty, fieldInfo);
        }
    }
}