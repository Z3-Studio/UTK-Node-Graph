using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    public class VariableList : VisualElement
    {
        public event Action OnDelete;

        private readonly List<Variable> targetList;
        private readonly ListViewBuilder<Variable, VariableView> customListView;
        private readonly ScriptableObject target;

        public VariableList(ScriptableObject so, List<Variable> source) : this("Variables", so, source) { }

        public VariableList(string label, ScriptableObject so, List<Variable> source, bool showFoldout = false, string tooptip = "")
        {
            target = so;
            targetList = source;

            Z3ListViewConfig listConfig = new()
            {
                listName = label,
                tooltip = tooptip,
                showAddBtn = true,
                showRemoveButton = false,
                selectable = false,
                showReordable = true,
                showFoldout = showFoldout,
                onMakeItem = OnMake,
                addEvent = () =>
                {
                    List<(string, Type)> types = TypeResolver.CachedVariables;
                    types.Add(("☆ Insert Title", typeof(Title)));
                    SelectorPopup<Type>.OpenWindow("New Variable", types, OnAddNewVariable);
                }
            };

            customListView = new ListViewBuilder<Variable, VariableView>(source, listConfig);

            // Add Visual Elements
            Add(customListView);

            customListView.OnBuildList += OnValidateNames;

            RegisterCallback<DetachFromPanelEvent>(Detach);

            void Detach(DetachFromPanelEvent e)
            {
                OnDelete = null;
            }
        }

        private void OnAddNewVariable(string _, Type type)
        {
            Variable variable = Variable.CreateVariable(target, type, targetList);

            if (type == typeof(Title))
            {
                variable.name = "- New Title";
            }

            customListView.Rebuild();
        }

        private VariableView OnMake()
        {
            VariableView visualElement = new();
            visualElement.OnDelete += OnDeleteVariable;
            visualElement.OnShowDependencies += OnShowDependencies;
            visualElement.OnDuplicateVariable += OnDuplicateVariable;
            visualElement.OnChangeVariable += OnChangeVariable;
            visualElement.OnChangeName += OnValidateNames;
            return visualElement;
        }

        private void OnValidateNames()
        {
            List<string> forbiddenList = targetList.GroupBy(v => v.name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            foreach (VariableView item in GetItems())
            {
                item.CheckInvalidName(forbiddenList);
            }
        }

        private void OnChangeVariable(Variable variable)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        private void OnDuplicateVariable(Variable variable)
        {
            Variable.CreateVariable(target, variable.OriginalType, targetList, variable.Name);
            customListView.Rebuild();
        }

        private void OnShowDependencies(Variable variable)
        {
            bool isReferenceVariable = target is GraphVariables;
            VariableDependenciesWindow.ShowDependencies(variable, isReferenceVariable);
        }
        private void OnDeleteVariable(Variable variable)
        {
            if (variable.OriginalType != typeof(Title))
            {
                // A duplicated graph keeps the same variable Guid, so match the object too
                // TODO: When copying GraphData or GraphVariables, we should generate new Guid for variables to avoid this issue.
                //       Will be necessary a manual command to link old Guid to new Guid for parameters using parameter name.
                //       This also will optimize the search here.
                int dependenciesCount = NodeGraphEditorUtils.GetAllParameters()
                    .Where(g => g.Key.GetVariables().Contains(variable))
                    .SelectMany(g => g.Value.Values)
                    .Sum(parameters => parameters.Count(p => p.Guid == variable.Guid));

                if (dependenciesCount > 0)
                {
                    int result = EditorUtility.DisplayDialogComplex($"Are you sure you want to delete '{variable.name}'?", $"There is a total of '{dependenciesCount}' in project", "Confirm", "Cancel", "See dependencies");
                    if (result == 1)
                        return;

                    if (result == 2)
                    {
                        VariableDependenciesWindow.ShowDependencies(variable, target is GraphVariables);
                        return;
                    }

                    // TODO: Unbind all parameters using this variable
                }
            }

            targetList.Remove(variable);
            customListView.Rebuild();

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

            OnDelete?.Invoke();
        }

        internal List<VariableView> GetItems() => customListView.GetElements();
    }
}