﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class ParameterView : VisualElement
    {
        [UIElement] private Label parameterName;
        [UIElement] private VisualElement variableSelectionContainer;
        [UIElement] private Label variableSelectionLabel;
        [UIElement] private VisualElement parameterSlot;
        [UIElement] private Toggle bindToggle;
        [UIElement] private VisualElement convertionContainer;
        [UIElement] private Label convertionLabel;

        private readonly PropertyField propertyField;
        private readonly IParameter parameterT;
        private readonly GraphData data;
        private readonly GraphSubAsset targetObject;
        private readonly string displayName;

        private Type GenericType => parameterT.GenericType;

        private const string SelfBind = "☆ Self Bind";
        private const string NewReferenceVariable = "☆ Promote Reference Variable";
        private const string NewLocalVariable = "☆ Promote Local Variable";

        public ParameterView(SerializedProperty property, FieldInfo fieldInfo)
        {
            targetObject = NodeGraphUtils.GetTarget<GraphSubAsset>(property);
            data = NodeGraphUtils.GetGraphData(targetObject);

            // TODO: Validate
            NodeGraphResources.ParameterVT.CloneTree(this);
            this.BindUIElements();

            // Remove preview
            parameterSlot.Clear();

            parameterT = property.GetValue<IParameter>();

            SerializedProperty serializedProp = property.FindPropertyRelative(Parameter<object>.ValueField);

            if (serializedProp != null) // Interface ex
            {
                propertyField = new PropertyField(serializedProp);
                propertyField.label = string.Empty;
                propertyField.style.width = new Length(100f, LengthUnit.Percent);
                parameterSlot.Add(propertyField);

                // Applies the attributes of the generic class to the generic argument
                FieldInfo relativeField = fieldInfo.FieldType.GetField(Parameter<object>.ValueField, ReflectionUtils.InstanceAccess);
                ApplyAttributes(serializedProp, propertyField, relativeField, fieldInfo);
            }

            displayName = property.displayName;
            parameterName.text = displayName;
            parameterName.tooltip = GenericType.Name;

            bindToggle.RegisterValueChangedCallback((e) => SetBinding(e.newValue));
            variableSelectionContainer.RegisterCallback<MouseDownEvent>(OpenVariablesSelection);

            // Set toggle
            bool hasBind = parameterT.IsBinding;
            bindToggle.SetValueWithoutNotify(hasBind);
            SetBinding(hasBind);

            this.RegisterUpdate(UpdateLabel);

            UpdateVariable();
        }

        private void UpdateLabel() // TODO: User event instead update
        {
            if (!bindToggle.value)
                return;
            // TODO: Check if the variable still existing and if is valid type

            if (!parameterT.IsBinding)
            {
                variableSelectionLabel.text = "Select a variable".AddRichTextColor(EditorStyle.DarkLabel);
            }
            else if (parameterT.IsSelfBind)
            {
                variableSelectionLabel.text = SelfBind;
            }
            else if (!parameterT.IsDefined) 
            {
                variableSelectionLabel.text = $"Missing variable, GUID: {parameterT.Guid}".AddRichTextColor(Color.red);
            }
            else
            {
                variableSelectionLabel.text = parameterT.Variable.name;
            }
        }

        private void SetBinding(bool bindingVariable)
        {
            variableSelectionContainer.style.SetDisplay(bindingVariable);
            parameterSlot.style.SetDisplay(!bindingVariable);

            if (!bindingVariable)
            {
                parameterT.Unbind();
                variableSelectionLabel.text = string.Empty;
                UpdateVariable();
            }
        }

        private void OpenVariablesSelection(MouseDownEvent _)
        {
            // Build variable list
            List<(string, Variable)> variables = new();

            variables.AddRange(CreatePath("Local Variables", data.LocalVariables));

            if (data.ReferenceVariables != null)
            {
                variables.AddRange(CreatePath("Reference Variables", data.ReferenceVariables.GetOriginalVariables()));
            }

            // Include Promote variable
            if (data.ReferenceVariables != null)
            {
                variables.Add((NewReferenceVariable, null));
            }

            variables.Add((NewLocalVariable, null));

            // Get Self Component
            bool canSelfBind = typeof(Component).IsAssignableFrom(GenericType) 
                || typeof(GameObject).IsAssignableFrom(GenericType) 
                || GenericType.IsInterface;

            if (canSelfBind)
            {
                variables.Add((SelfBind, null));
            }

            SelectorPopup<Variable>.OpenWindow("Select Variable", variables, OnBind);
        }

        private void OnBind(string name, Variable variable)
        {
            if (variable == null)
            {
                if (name == SelfBind)
                {
                    parameterT.SelfBind();
                }
                else if (name == NewLocalVariable)
                {
                    variable = Variable.CreateVariable(GenericType, data.LocalVariables, displayName.Replace(" ", string.Empty));
                    NodeGraphWindow.ForceRedrawVariables(data);
                }
                else if (name == NewReferenceVariable && data.ReferenceVariables != null)
                {
                    variable = Variable.CreateVariable(GenericType, data.ReferenceVariables.GetOriginalVariables(), displayName.Replace(" ", string.Empty));
                    NodeGraphWindow.ForceRedrawVariables(data);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            if (variable != null)
            {
                parameterT.Bind(variable);
            }

            UpdateVariable();

            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
        }

        private void UpdateVariable()
        {
            if (parameterT.IsBinding && parameterT.IsDefined)
            {
                Converter getConverter = TypeResolver.GetGetConverterType(parameterT, parameterT.Variable);

                // TODO: null check
                if (getConverter.type != ConvertionType.IsAssignableFrom)
                {
                    Converter setConverter = TypeResolver.GetSetConverterType(parameterT, parameterT.Variable);

                    convertionContainer.style.SetDisplay(true);

                    string get = $"Get: {getConverter.Description}\nVariable: {getConverter.OutType?.Name}\nParameter: {getConverter.InType?.Name}";
                    string set = $"Set: {setConverter.Description}\nVariable: {setConverter.OutType?.Name}\nParameter: {setConverter.InType?.Name}";
                    convertionContainer.tooltip = $"{get}\n\n{set}";

                    string label = string.Empty;
                    if (getConverter.type == ConvertionType.TypeConverter)
                    {
                        label += " get;";
                    }

                    if (setConverter.type == ConvertionType.TypeConverter)
                    {
                        label += " set;";
                    }

                    convertionLabel.text = label;
                    return;
                }
            }

            convertionContainer.style.SetDisplay(false);
        }

        private IEnumerable<(string, Variable)> CreatePath(string title, List<Variable> list)
        {
            List<(string, Variable)> variables = new();
            foreach (Variable variable in list)
            {
                if (variable.OriginalType == typeof(Title))
                    continue;


                Converter getConvertion = TypeResolver.GetGetConverterType(parameterT, variable);
                if (getConvertion.type == ConvertionType.Null)
                {

                    getConvertion = TypeResolver.GetSetConverterType(parameterT, variable);
                    if (getConvertion.type == ConvertionType.Null)
                        continue;
                }

                string newLabel = $"{title}/";
                if (getConvertion.type == ConvertionType.IsAssignableFrom)
                {
                    newLabel += variable.Name;
                }
                else
                {
                    newLabel += $"{variable.Name} <b>({getConvertion.type})";
                }

                variables.Add((newLabel, variable));
                
            }

            return variables;
        }

        [Obsolete("Work around, try to improve it")]
        private static void ApplyAttributes(SerializedProperty serializedProperty, VisualElement propertyField, FieldInfo relativeFieldInfo, FieldInfo fieldInfo)
        {
            List<Z3VisualElementAttribute> attributes = fieldInfo.GetCustomAttributes<Z3VisualElementAttribute>().ToList();
            if (attributes.Count == 0)
                return;

            propertyField.ExecuteWhenAttach(() =>
            {
                foreach (Z3VisualElementAttribute attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    if (UIBuilderCache.AttributeDrawers.TryGetValue(attributeType, out IZ3AttributeDrawer drawer))
                    {
                        drawer.Init(serializedProperty, propertyField, fieldInfo, attribute);
                        if (drawer.CanDraw())
                            continue;

                        drawer.Init(serializedProperty, propertyField, relativeFieldInfo, attribute);
                        drawer.Draw();
                    }
                }
            });
        }
    }
}