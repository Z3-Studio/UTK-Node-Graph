using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    /// <summary> Lists the sub assets whose parameters use a variable </summary>
    public class VariableDependenciesWindow : EditorWindow
    {
        [Serializable]
        private class Entry
        {
            public GraphData graph;
            public GraphSubAsset subAsset;
            public string parameterTypes; // Type is not serializable
            public int count;
        }

        private const string WindowName = "Variable Dependencies";

        // Note: Serialize to survive domain reload
        [SerializeField] private Variable variable;
        [SerializeField] private bool isReferenceVariable;
        [SerializeField] private List<Entry> entries = new();

        [UIElement] private Label variableLabel;
        [UIElement] private Label typeLabel;
        [UIElement] private Label scopeLabel;
        [UIElement] private Label guidLabel;
        [UIElement] private Label graphsLabel;
        [UIElement] private Label dependenciesLabel;
        [UIElement] private VisualElement entriesContainer;

        public static void ShowDependencies(Variable variable, bool isReferenceVariable)
        {
            VariableDependenciesWindow window = GetWindow<VariableDependenciesWindow>(WindowName);
            window.Populate(variable, isReferenceVariable);
        }

        private void Populate(Variable variable, bool isReferenceVariable)
        {
            this.variable = variable;
            this.isReferenceVariable = isReferenceVariable;
            entries.Clear();

            foreach ((GraphData graph, Dictionary<GraphSubAsset, List<IParameter>> subAssets) in NodeGraphEditorUtils.GetAllParameters())
            {
                // A duplicated graph keeps the same variable Guid, so match the object too
                if (!graph.GetVariables().Contains(variable))
                    continue;

                foreach ((GraphSubAsset subAsset, List<IParameter> parameters) in subAssets)
                {
                    List<IParameter> matches = parameters.Where(p => p.Guid == variable.Guid).ToList();
                    if (matches.Count == 0)
                        continue;

                    string parameterTypes = string.Join(", ", matches.Select(p => p.GenericType?.ToString() ?? "-").Distinct());
                    entries.Add(new Entry { graph = graph, subAsset = subAsset, parameterTypes = parameterTypes, count = matches.Count });
                }
            }

            Rebuild();
        }

        private void CreateGUI()
        {
            NodeGraphResources.VariableDependenciesWindowVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            Rebuild();
        }

        private void Rebuild()
        {
            entriesContainer.Clear();

            if (!variable)
            {
                variableLabel.text = "Null".ToItalic();
                typeLabel.text = string.Empty;
                scopeLabel.text = string.Empty;
                guidLabel.text = string.Empty;
                graphsLabel.text = string.Empty;
                dependenciesLabel.text = string.Empty;
                return;
            }

            List<Entry> validEntries = entries
                .Where(e => e.subAsset) // TODO: Review it, maybe is not necessary (Sub assets deleted after the search?)
                .OrderBy(e => e.graph.name)
                .ThenBy(e => e.subAsset.name)
                .ToList();

            variableLabel.text = variable.Name;
            typeLabel.text = variable.OriginalType?.ToString() ?? "Missing type";
            scopeLabel.text = isReferenceVariable ? "Reference" : "Local";
            guidLabel.text = variable.Guid;
            graphsLabel.text = validEntries.Select(e => e.graph).Distinct().Count().ToString();
            dependenciesLabel.text = validEntries.Sum(e => e.count).ToString();

            if (validEntries.Count == 0)
                return;

            Columns columns = new()
            {
                new Column
                {
                    name = "graph",
                    title = "Graph",
                    width = 200,
                    makeCell = MakeCell,
                    bindCell = (e, i) => BindCell(e, validEntries[i], validEntries[i].graph.name)
                },
                new Column
                {
                    name = "sub-asset",
                    title = "Sub Asset",
                    width = 400,
                    makeCell = MakeCell,
                    bindCell = (e, i) => BindCell(e, validEntries[i], validEntries[i].subAsset.name)
                },
                new Column
                {
                    name = "parameter-type",
                    title = "Parameter Type",
                    width = 300,
                    makeCell = MakeCell,
                    bindCell = (e, i) => BindCell(e, validEntries[i], validEntries[i].parameterTypes)
                },
                new Column
                {
                    name = "dependencies",
                    title = "Dependencies",
                    width = 100,
                    makeCell = MakeCell,
                    bindCell = (e, i) => BindCell(e, validEntries[i], validEntries[i].count.ToString())
                }
            };

            MultiColumnListView table = new(columns)
            {
                itemsSource = validEntries
            };

            table.style.flexGrow = 1;
            entriesContainer.Add(table);
        }

        private static VisualElement MakeCell()
        {
            Label label = new();
            label.style.flexGrow = 1;
            label.style.alignSelf = Align.Stretch;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.paddingLeft = 6;
            label.RegisterCallback<ClickEvent>(_ => Ping(label.userData as Entry));
            return label;
        }

        private static void BindCell(VisualElement cell, Entry entry, string text)
        {
            Label label = (Label)cell;
            label.userData = entry;
            label.text = text;
        }

        private static void Ping(Entry entry)
        {
            // Can be deleted while the window is open
            if (!entry.subAsset)
                return;

            EditorGUIUtility.PingObject(entry.subAsset);
        }
    }
}
