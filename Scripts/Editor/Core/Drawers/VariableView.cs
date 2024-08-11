using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public class VariableView : VisualElement
    {
        [UIElement] private TextField variableName;
        [UIElement] private VisualElement propertyContainer;
        [UIElement] private Button actionsButton;

        public event Action OnChangeName;
        public event Action<Variable> OnDelete;
        public event Action<Variable> OnChangeVariable;
        public event Action<Variable> OnDuplicateVariable;

        public Variable Variable { get; private set; }

        private SerializedProperty serializedProperty;

        public VariableView(SerializedProperty serializedProperty) : this()
        {
            Variable = serializedProperty.GetValue<Variable>();
            BuildElement();
        }

        public VariableView()
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;
        }

        public void SetElement(Variable variable)
        {
            Variable = variable;
            BuildElement();
        }

        public void CheckInvalidName(List<string> invalidNames)
        {
            bool Invalid = invalidNames.Contains(Variable.name);
            style.backgroundColor = Invalid ? new Color(1f, 0f, 0f, .6f) : Color.clear;
        }

        private void BuildElement()
        {
            NodeGraphResources.VariableVT.CloneTree(this);
            this.BindUIElements();

            Validate();

            // Variable Name
            variableName.value = Variable.name;

            variableName.RegisterCallback<BlurEvent>(evt =>
            {
                OnChangeName?.Invoke();
                OnChangeVariable?.Invoke(Variable);
            });

            variableName.RegisterValueChangedCallback((e) =>
            {
                string newValue = e.newValue;
                if (newValue.Contains("/"))
                {
                    // Avoid create paths when open the bind section of the ParameterView
                    Debug.Log("Variable names with '/' is not allowed");
                    newValue = newValue.Replace("/", string.Empty);
                    variableName.value = newValue;
                }
                Variable.name = newValue;
            });

            Type type = Variable.OriginalType;
            if (type == typeof(Title))
            {
                // Draw as title
                actionsButton.clicked += () => OnDelete?.Invoke(Variable);
                propertyContainer.style.SetDisplay(false);
            }
            else
            {
                DrawAsVariable(type);
            }
        }

        private void DrawAsVariable(Type type)
        {
            string typeName = type != null ? type.Name : "Null Type";

            // Variable Value
            FieldInfo field = Variable.GetType().GetField(nameof(Variable.value));

            IBaseFieldReader baseField = EditorBuilder.GetElement(type);
            VisualElement valueField = baseField.VisualElement;

            // Check if is a field or serialized property
            if (baseField.TwoWay)
            {
                // Bind
                baseField.CreateGetSet
                (
                    () => Variable.value,
                    newValue => Variable.value = newValue
                );

                // Save changes
                baseField.OnChangeValue += () =>
                {
                    Variable.value = baseField.Value;
                    if (serializedProperty != null)
                    {
                        EditorUtility.SetDirty(serializedProperty.serializedObject.targetObject);
                    }
                    else
                    {
                        OnChangeVariable?.Invoke(Variable);
                    }
                };

                // Remove Label of the value field
                Label label = valueField.Q<Label>();
                if (label != null && label.parent != null)
                {
                    label.parent.Remove(label);
                }
            }
            else
            {
                // Create buttons for serialized properties
                if (Variable.value == null)
                {
                    valueField = new Button(() =>
                    {
                        Variable.value = Activator.CreateInstance(Variable.OriginalType);

                    }) { text = "Create Instance" };
                }
                else
                {
                    valueField = new Button(() =>
                    {
                        PropertyWindow.OpenWindow(Variable.Name, Variable.value);

                    }) { text = "Show Instance" };
                }
            }

            valueField.style.SetAsExpanded();
            valueField.style.marginLeft = 10;

            // Menu Options
            actionsButton.style.backgroundImage = (Texture2D)EditorIcons.GetTexture(IconType.Cog);
            actionsButton.style.backgroundSize = new BackgroundSize(Length.Percent(75), Length.Percent(75));
            actionsButton.text = string.Empty;

            actionsButton.clicked += () =>
            {
                DropdownMenu menu = new DropdownMenu();
                menu.AppendAction(typeName, null, DropdownMenuAction.Status.Disabled);

                menu.AppendAction("Duplicate", action =>
                {
                    OnDuplicateVariable.Invoke(Variable);
                });

                menu.AppendAction($"Change Type", action =>
                {
                    List<(string, Type)> types = TypeResolver.CachedVariables;
                    SelectorPopup<Type>.OpenWindow("Select New Type", types, SetType, actionsButton.contentRect.position);
                });

                menu.AppendAction("Delete", action =>
                {
                    OnDelete.Invoke(Variable);
                });

                Rect rect = new Rect(Event.current.mousePosition.x , Event.current.mousePosition.y, 0, 0);
                menu.DisplayEditorMenu(rect);
            };

            propertyContainer.Add(valueField);
        }

        private void SetType(string _, Type selectedType)
        {
            Variable.SetType(selectedType);

            Clear();
            BuildElement();

            OnChangeVariable?.Invoke(Variable);
        }

        private void Validate()
        {
            Type baseType = Variable.OriginalType;
            Type valueType = Variable.value?.GetType();

            // Unspecified type in both side, impossible to fix
            if (baseType == null && valueType == null)
            {
                Debug.LogError("Parameter Type and value is null");
                Variable.type = typeof(Object).AssemblyQualifiedName;
                Variable.value = null;
                return;
            }

            // If current value is null
            if (valueType == null)
            {
                // Check if the type is nullable, fix it if is neccessary
                if (!baseType.IsNullable())
                {
                    Debug.LogError($"Value typeof '{baseType.Name}' is not nullable, fixing...");
                    Variable.value = baseType.GetDefaultValueForType();
                }

                return;
            }

            // If baseType is null, try to fix it
            if (baseType == null)
            {
                Debug.LogError("Type is null, trying to fix...");
                baseType = valueType;
            }

            // Weird value
            if (!baseType.IsAssignableFrom(valueType))
            {
                Debug.LogError($"Invalid value type of '{valueType}', the base type is '{baseType}'");
                Variable.value = baseType.GetDefaultValueForType();
            }
        }
    }
}