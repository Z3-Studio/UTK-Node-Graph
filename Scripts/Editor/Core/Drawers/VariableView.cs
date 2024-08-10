using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public class VariableView : VisualElement
    {
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
            Validate();

            // Variable Name
            TextField nameText = new TextField();
            nameText.value = Variable.name;
            nameText.style.SetAsExpanded();

            nameText.RegisterCallback<BlurEvent>(evt =>
            {
                OnChangeName?.Invoke();
                OnChangeVariable?.Invoke(Variable);
            });

            nameText.RegisterValueChangedCallback((e) =>
            {
                string newValue = e.newValue;
                if (newValue.Contains("/"))
                {
                    // Avoid create paths when open the bind section of the ParameterView
                    Debug.Log("Variable names with '/' is not allowed");
                    newValue = newValue.Replace("/", string.Empty);
                    nameText.value = newValue;
                }
                Variable.name = newValue;
            });

            Add(nameText);

            Type type = Variable.OriginalType;
            if (type == typeof(Title))
            {
                DrawAsTitle();
            }
            else
            {
                DrawAsVariable(type);
            }

            // Temp
            //SerializedProperty property = new SerializedProperty(parameter); // IMGUIContainer?
            //PropertyField propertyField = new PropertyField(property);
            //Add(propertyField);
        }

        private void DrawAsTitle()
        {
            ToolbarButton button = new(() => OnDelete?.Invoke(Variable));
            button.text = "x";

            button.style.paddingLeft = 5;
            button.style.unityFontStyleAndWeight = FontStyle.Italic;
            EditorStyle.SetSmallEditorButton(button.style);

            Add(button);
        }

        private void DrawAsVariable(Type type)
        {
            string typeName = type != null ? type.Name : "Null Type";

            // Variable Value
            FieldInfo field = Variable.GetType().GetField(nameof(Variable.value));

            IBaseFieldReader baseField = EditorBuilder.GetElement(type);
            VisualElement valueField = baseField.VisualElement;

            if (baseField.TwoWay)
            {
                baseField.CreateGetSet(() =>
                {
                    return Variable.value;
                }, newValue => Variable.value = newValue);

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

            // Menu
            ToolbarMenu toolbarMenu = new ToolbarMenu();
            VisualElement iconElement = toolbarMenu.ElementAt(1);

            iconElement.style.backgroundImage = (Texture2D)EditorIcons.GetTexture(IconType.Cog);
            iconElement.style.right = 1.5f;

            EditorStyle.SetSmallEditorButton(toolbarMenu.style);
            toolbarMenu.style.marginRight = 1f;

            // Menu Options
            DropdownMenu menu = toolbarMenu.menu;
            menu.AppendAction(typeName, null, DropdownMenuAction.Status.Disabled);

            menu.AppendAction("Duplicate", action =>
            {
                OnDuplicateVariable.Invoke(Variable);
            });

            menu.AppendAction("Delete", action =>
            {
                OnDelete.Invoke(Variable);
            });

            menu.AppendAction($"Change Type", action =>
            {
                List<(string, Type)> types = TypeResolver.CachedVariables;
                SelectorPopup<Type>.OpenWindow("Select New Type", types, SetType, worldBound.center);
            });

            // Build Row
            Add(valueField);
            Add(toolbarMenu);
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