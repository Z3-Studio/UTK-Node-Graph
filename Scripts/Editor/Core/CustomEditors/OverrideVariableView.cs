using System;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.UIBuilder.Core;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    public class OverrideVariableView : VisualElement, IBindElement<Variable>
    {
        [UIElement] private Label variableName;
        [UIElement] private VisualElement propertyContainer;
        [UIElement] private Button actionsButton;

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

        public void Bind(Variable item, int index)
        {
            // TODO: Improve it
        }

        public void SetElement(Func<Variable> getVariable, Func<OverrideVariable> getOverrideVariable)
        {
            GetVariable = getVariable;

            GetOverrideVariable = getOverrideVariable;
            BuildElement();
        }

        private void BuildElement()
        {
            NodeGraphResources.OverrideVariableVT.CloneTree(this);
            this.BindUIElements();

            variableName.text = Variable.name;

            Type type = Type.GetType(Variable.type);
            if (type == typeof(Title))
            {
                propertyContainer.style.SetDisplay(false);
                actionsButton.style.SetDisplay(false);
                return;
            }

            // Menu

            IBaseFieldReader baseField;
            if (OverrideVariable)
            {
                FieldInfo field = OverrideVariable.GetType().GetField(nameof(OverrideVariable.value));

                baseField = EditorBuilder.GetElement(OverrideVariable, field, type);
                baseField.OnValueChangedAfterBlur += OnUpdateValue;

                actionsButton.text = "-";
            }
            else
            {
                FieldInfo field = Variable.GetType().GetField(nameof(Variable.value));
                baseField = EditorBuilder.GetElement(Variable, field, type);

                baseField.VisualElement.SetEnabled(false);
                actionsButton.text = "+";
            }

            baseField.SetLabel(string.Empty);

            // Set Style
            propertyContainer.Add(baseField.VisualElement);
        }

        [UIElement("actions-button")]
        private void Switch()
        {
            if (OverrideVariable)
            {
                OnRemove.Invoke(OverrideVariable);
            }
            else
            {
                OverrideVariable newOverrideVariable = new OverrideVariable()
                {
                    guid = Variable.guid,
                    value = Variable.value
                };

                OnCreate(newOverrideVariable);
            }

            // Rebuild
            Clear();
            BuildElement();
        }
    }
}