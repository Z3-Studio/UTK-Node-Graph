using System;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

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
            IBaseFieldReader baseField = EditorBuilder.GetElement(variable, propertyInfo, type);
            VisualElement valueField;

            if (baseField.TwoWay)
            {
                baseField.SetLabel(string.Empty);
                valueField = baseField.VisualElement;
            }
            else
            {
                valueField = new Button(() =>
                {
                    PropertyWindow.OpenWindow(variable.Name, variable.Value, type);
                })
                { text = "Show Instance" };
            }

            // Set Style
            valueField.style.SetAsExpanded();
            Add(valueField);
        }
    }
}