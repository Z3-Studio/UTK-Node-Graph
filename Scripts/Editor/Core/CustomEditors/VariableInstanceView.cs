using System;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using System.Reflection;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Editor
{
    public class VariableInstanceView : VisualElement, IBindElement<VariableInstance>
    {
        public VariableInstanceView()
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceAround;
        }

        public void Bind(VariableInstance variable, int index)
        {
            // Variable Name
            Label nameText = new Label();
            nameText.text = variable.Name;

            // Build Row
            nameText.style.SetAsExpanded();
            Add(nameText);

            Type type = variable.OriginalType;
            if (type == typeof(Title))
                return;

            // Field
            PropertyInfo propertyInfo = variable.GetType().GetProperty(nameof(variable.Value));
            IBaseFieldReader field = EditorBuilder.GetElement(variable, propertyInfo, type);
            VisualElement valueField = field.VisualElement;

            // Remove Label of the value field
            Label label = valueField.Q<Label>();
            if (label != null && label.parent != null)
            {
                label.parent.Remove(label);
            }

            // Set Style
            valueField.style.SetAsExpanded();
            Add(valueField);
        }
    }
}