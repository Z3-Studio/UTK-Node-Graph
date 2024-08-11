using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
                onBind = OnBind,
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
            Variable.CreateVariable(type, targetList);
            customListView.Rebuild();

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private void OnBind(VisualElement visualElement, int i)
        {
            Variable variable = targetList[i];
            VariableView variableView = visualElement as VariableView;

            if (variableView.Variable != null) // Avoid change values when reorder
                return;

            variableView.SetElement(variable);
        }

        private VariableView OnMake()
        {
            VariableView visualElement = new();
            visualElement.OnDelete += OnDeleteVariable;
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
            Variable.CreateVariable(variable.OriginalType, targetList, variable.Name);
            customListView.Rebuild();

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private void OnDeleteVariable(Variable variable)
        {
            if (variable.OriginalType != typeof(Title))
            {
                // TODO: Local List: Get this GraphData and validate all dependencies
                // TODO: Reference List: Get all GraphData in project using this graph using the GraphVariable list and validate all dependencies

                int ok = EditorUtility.DisplayDialogComplex($"Are you sure you want to delete '{variable.name}'?", "TODO: Display all dependency of the local list and reference list", "Confirm", "Cancel", "See dependencies");
                if (ok == 1)
                    return;

                if (ok == 2)
                {
                    Debug.Log("TODO: Display Validator Window");
                    return;
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