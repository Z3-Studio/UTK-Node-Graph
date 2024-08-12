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
                onMakeItem = () => new OverrideVariableView()
            };

            customListView = new ListViewBuilder<Variable, OverrideVariableView>(parentVariables, listConfig);
            customListView.onBind += OnBind;
            Add(customListView);
        }

        internal List<OverrideVariableView> GetItems() => customListView.GetElements();

        private void OnBind(OverrideVariableView view, Variable _, int i)
        {
            view.OnRemove += OnRevert;
            view.OnCreate += OnCreate;
            view.OnUpdateValue += OnChangeValue;

            Func<Variable> variable = () => targetList[i];
            Func<OverrideVariable> overrideVariable = () => overrideList.FirstOrDefault(ov => ov == targetList[i]);

            view.SetElement(variable, overrideVariable);
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