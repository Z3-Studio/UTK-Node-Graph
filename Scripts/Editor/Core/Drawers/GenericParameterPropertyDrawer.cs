using UnityEditor;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    [System.Obsolete("Is not working")]
    [CustomPropertyDrawer(typeof(GenericParameter<>), true)]
    public class GenericParameterPropertyDrawer : Z3PropertyDrawer<IParameter>
    {
        protected override VisualElement CreateVisualElement()
        {
            return new GenericParameterView(SerializedProperty, fieldInfo);
        }
    }
}
