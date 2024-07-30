using System;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using System.Reflection;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class OverrideVariableView : VisualElement
    {
        public event Action<OverrideVariable> OnCreate;
        public event Action<OverrideVariable> OnRemove;
        public event Action OnUpdateValue;

        public Variable Variable { get => GetVariable(); }
        private OverrideVariable OverrideVariable { get => GetOverrideVariable(); }

        private Func<OverrideVariable> GetOverrideVariable { get; set; }
        private Func<Variable> GetVariable { get; set; }

        public OverrideVariableView()
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceAround;
        }

        public void SetElement(Func<Variable> getVariable, Func<OverrideVariable> getOverrideVariable)
        {
            GetVariable = getVariable;

            GetOverrideVariable = getOverrideVariable;
            BuildElement();
        }

        private void BuildElement()
        {
            // Variable Name
            Label nameText = new Label();
            nameText.text = Variable.name;

            // Build Row
            nameText.style.SetAsExpanded();
            Add(nameText);

            Type type = Type.GetType(Variable.type);
            if (type == typeof(Title))
                return;

            // Menu
            Button revertButton = new Button();

            VisualElement valueField;
            if (OverrideVariable)
            {
                FieldInfo field = OverrideVariable.GetType().GetField(nameof(OverrideVariable.value));

                IBaseFieldReader baseField = EditorBuilder.GetElement(OverrideVariable, field, type);
                valueField = baseField.VisualElement;
                baseField.OnChangeValue += OnUpdateValue;

                ButtonAsOverride(revertButton);
            }
            else
            {
                FieldInfo field = Variable.GetType().GetField(nameof(Variable.value));
                IBaseFieldReader baseField = EditorBuilder.GetElement(Variable, field, type);
                valueField = baseField.VisualElement;

                valueField.SetEnabled(false);
                ButtonAsVariable(revertButton);
            }

            // Remove Label of the value field
            Label label = valueField.Q<Label>();
            if (label != null && label.parent != null)
            {
                label.parent.Remove(label);
            }

            // Set Style
            valueField.style.SetAsExpanded();
            Add(valueField);

            EditorStyle.SetSmallEditorButton(revertButton.style);
            Add(revertButton);
        }

        private void ButtonAsOverride(Button revertButton)
        {
            revertButton.style.backgroundImage = (Texture2D)EditorIcons.GetTexture(UIBuilder.IconType.Minus);
            revertButton.clicked += () =>
            {
                OnRemove.Invoke(OverrideVariable);
                Rebuild();
            };
        }

        private void ButtonAsVariable(Button revertButton)
        {
            revertButton.style.backgroundImage = (Texture2D)EditorIcons.GetTexture(UIBuilder.IconType.Plus);
            revertButton.clicked += () =>
            {
                OverrideVariable newOverrideVariable = new OverrideVariable()
                {
                    guid = Variable.guid,
                    value = Variable.value
                };

                OnCreate(newOverrideVariable);
                Rebuild();
            };
        }

        private void Rebuild()
        {
            Clear();
            BuildElement();
        }
    }
}