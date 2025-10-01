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

        private void OnDeleteVariable(Variable variable)
        {
            if (variable.OriginalType != typeof(Title))
            {
                Dictionary<GraphData, Dictionary<GraphSubAsset, List<IParameter>>> allParameters = NodeGraphEditorUtils.GetAllParameters();
                Dictionary<GraphData, Dictionary<GraphSubAsset, List<IParameter>>> graphDependencies = new();

                int dependenciesCount = 0;
                foreach ((GraphData data, Dictionary<GraphSubAsset, List<IParameter>> subAssets) in allParameters)
                {
                    Dictionary<GraphSubAsset, List<IParameter>> subAssetDependencies = new();

                    foreach ((GraphSubAsset subAsset, List<IParameter> parameter) in subAssets)
                    {
                        List<IParameter> parametersWithDependencies = parameter.Where(p => p.Guid == variable.Guid).ToList();
                        if (parametersWithDependencies.Count == 0)
                            continue;

                        subAssetDependencies[subAsset] = parametersWithDependencies;
                        dependenciesCount += parametersWithDependencies.Count;
                    }

                    if (subAssetDependencies.Count == 0)
                        continue;

                    graphDependencies[data] = subAssetDependencies;
                }

                if (dependenciesCount > 0)
                {
                    int result = EditorUtility.DisplayDialogComplex($"Are you sure you want to delete '{variable.name}'?", $"There is a total of '{dependenciesCount}' in project", "Confirm", "Cancel", "See dependencies");
                    if (result == 1)
                        return;

                    if (result == 2)
                    {
                        EditorUtility.DisplayDialog("Not implemented", "TODO: Display all dependency of the local list and reference list. HOWEVER, check the console log", "ok");

                        foreach ((GraphData data, Dictionary<GraphSubAsset, List<IParameter>> subAssets) in graphDependencies)
                        {
                            foreach ((GraphSubAsset subAsset, List<IParameter> parameter) in subAssets)
                            {
                                Debug.Log($"GraphSubAsset {subAsset}, Dependencies = {parameter.Count}", subAsset);
                            }
                        }

                        return;
                    }
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