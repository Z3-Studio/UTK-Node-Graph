using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public class OverrideVariableList : VisualElement
    {
        private List<Variable> targetList;
        private List<OverrideVariable> overrideList;
        private Object origin;

        private ListViewBuilder<Variable, OverrideVariableView> customListView;

        public OverrideVariableList(Object origin, List<OverrideVariable> overrideVariables, List<Variable> parentVariables)
        {
            targetList = parentVariables;
            overrideList = overrideVariables;
            this.origin = origin;

            Z3ListViewConfig listConfig = new()
            {
                listName = "Override Variables",
                showAddBtn = false,
                showRemoveButton = false,
                showReordable = false,
                showFoldout = false,
                selectable = false,
                onMakeItem = OnMake,
                onBind = OnBind
            };

            customListView = new ListViewBuilder<Variable, OverrideVariableView>(parentVariables, listConfig);
            Add(customListView);
        }

        internal List<OverrideVariableView> GetItems() => customListView.GetElements();

        private OverrideVariableView OnMake()
        {
            OverrideVariableView visualElement = new();
            visualElement.OnRemove += OnRevert;
            visualElement.OnCreate += OnCreate;
            visualElement.OnUpdateValue += OnChangeValue;
            return visualElement;
        }

        private void OnBind(VisualElement visualElement, int i)
        {
            OverrideVariableView variableElement = visualElement as OverrideVariableView;

            Func<Variable> variable = () => targetList[i];
            Func<OverrideVariable> overrideVariable = () => overrideList.FirstOrDefault(ov => ov == targetList[i]);

            variableElement.SetElement(variable, overrideVariable);
        }

        private void OnCreate(OverrideVariable newOverrideVariable)
        {
            overrideList.Add(newOverrideVariable);
            OnChangeValue();
        }

        private void OnRevert(OverrideVariable overrideVariable)
        {
            overrideList.Remove(overrideVariable);
            OnChangeValue();
        }

        private void OnChangeValue()
        {
            EditorUtility.SetDirty(origin);
        }
    }
}