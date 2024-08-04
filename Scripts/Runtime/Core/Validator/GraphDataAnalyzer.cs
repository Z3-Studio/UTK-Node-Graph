using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Z3.Utils;
using Z3.Utils.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    /// <summary> Used to diagnose errors and anomalies </summary>
    public class GraphDataAnalyzer
    {
        public GraphData GraphData { get; }
        public List<IssueDetail> Issues { get; } = new(); // Maybe ObservableCollection
        public bool HasErrors => Issues.Count > 0;
        public int IssuesCount => Issues.Count;

        public Dictionary<string, VariableAnalyzer> VariableDependencies { get; } = new();

        public GraphDataAnalyzer(GraphData graphData)
        {
            GraphData = graphData;
            Validate();

            if (Issues.Count == 0)
                return;

            string log = $"The graph name of `{GraphData.name}' has {Issues.Count.ToStringBold()} errors. Open Validator to see actions";
            Debug.LogError(log, GraphData);
        }

        public void Refresh()
        {
            Validate();
        }

        public List<GraphSubAsset> GetDirectyDependencies(GraphSubAsset asset)
        {
            return ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset)
                .SelectMany(s => s.SubAssets.OfType<GraphSubAsset>())
                .ToList();
        }

        private void Validate()
        {
            VariableDependencies.Clear();
            Issues.Clear();

            // 1. Parameters
            Dictionary<string, Variable> variables = GraphData.GetVariables().ToDictionary(v => v.guid, v => v);

            string selfBind = Parameter<object>.SelfBind;
            VariableDependencies[selfBind] = new VariableAnalyzer(selfBind.ToItalic(), " - ", VariableScope.SelfBind);

            foreach ((string guid, Variable variable) in variables)
            {
                VariableScope scope = GraphData.LocalVariables.Any(v => v == variable) ? VariableScope.Local : VariableScope.Reference;
                VariableDependencies[guid] = new VariableAnalyzer(variable, scope);
            }

            foreach (GraphSubAsset asset in GraphData.SubAssets)
            {
                if (asset == null) // TODO: Review it
                    continue;

                asset.GetAllFieldValuesTypeOf<IParameter>().ForEach(p =>
                {
                    if (!p.IsBinding)
                        return;

                    string parameterGuid = p.Guid;

                    // Missing Dependency
                    if (!VariableDependencies.ContainsKey(parameterGuid))
                    {
                        VariableDependencies[parameterGuid] = new("Missing".AddRichTextColor(Color.red), parameterGuid, VariableScope.Undefined);
                    }

                    VariableDependencies[parameterGuid].AddDependency(asset, p);

                    // Validation is not required, even though this may cause errors. This is because the object can add and remove components.
                    if (p.IsSelfBind)
                        return;

                    // Check if the variable exist and the type matches
                    if (variables.TryGetValue(parameterGuid, out Variable variable) && TypeResolver.CanConvert(p, variable))
                    {
                        p.Bind(variable);
                    }
                    else
                    {
                        p.Invalid();

                        string log = $"Parameter type of '{p.GenericType}' has an invalid binding.\nVariable GUID: {parameterGuid.AddRichTextColor(Color.red)}\nAsset type '{asset.GetType().Name}' named as '{asset.name.AddRichTextColor(Color.yellow)}'";

                        // asset
                        AddAnalysis(asset, AnalysisType.MissingBinding, log, AnalysisCriticality.Warning);
                    }
                });
            }

            // 2. Check for corrupted or missing sub-assets
            string path = AssetDatabase.GetAssetPath(GraphData);
            int corruptedObjects = AssetDatabase.GetScriptableObjectsWithMissingScriptCount(path);
            if (corruptedObjects > 0)
            {
                AddAnalysis(null, AnalysisType.CorruptedAsset, $"Found '{corruptedObjects}' assets with Missing Scripts");
            }

            // 3. Find null elements
            for (int i = 0; i < GraphData.SubAssets.Count; i++)
            {
                if (GraphData.SubAssets[i] == null)
                {
                    AddAnalysis(null, AnalysisType.NullSubAsset, $"SubAssetList has null element at position {i.ToStringBold().AddRichTextColor(Color.red)}");
                }
            }

            // 4. Check missing nodes
            List<GraphSubAsset> subAssetList = EditorUtils.GetAllSubAssets<GraphSubAsset>(GraphData).ToList();
            List<Node> nodeList = subAssetList.OfType<Node>().ToList();

            foreach (Node node in nodeList)
            {
                if (!GraphData.SubAssets.Contains(node))
                {
                    AddAnalysis(node, AnalysisType.MissingNode, $"Node '{node.name.AddRichTextColor(Color.red)}' is not present in the SubAssetList");
                }
            }

            // 5. Check for null items in sub-assets
            foreach (GraphSubAsset subAsset in GraphData.SubAssets)
            {
                if (!subAsset)
                    continue;

                CheckForNullSubItems(subAsset);
            }

            // 6. Check for missing references
            foreach (GraphSubAsset subAsset in subAssetList)
            {
                if (subAsset is Node || !subAsset)
                    continue;

                GraphSubAsset foundReference = null;
                foreach (Node node in nodeList)
                {
                    foundReference = ContainsSubAssetInLists(node, subAsset, false);
                    if (foundReference)
                        break;
                }

                if (foundReference == null)
                {
                    AddAnalysis(subAsset, AnalysisType.MissingSubAsset, $"SubAsset '{subAsset.name.AddRichTextColor(Color.red)}' is not being referenced");
                }
            }

            // 7. Check if StartNode is set
            if (GraphData.StartNode == null && GraphData.SubAssets.Any(s => s is Node node && node.StartableNode))
            {
                AddAnalysis(null, AnalysisType.Other, "StartNode is not set", AnalysisCriticality.Warning);
            }

            // 8. Repeted items
            int distincCount = GraphData.SubAssets.Distinct().Count();
            if (distincCount < GraphData.SubAssets.Count)
            {
                AddAnalysis(null, AnalysisType.Other, "Repeated items", AnalysisCriticality.Error);
            }
        }

        private void AddAnalysis(GraphSubAsset context, AnalysisType errorType, string description, AnalysisCriticality criticalityLevel = AnalysisCriticality.Error)
        {
            IssueDetail newIssue = new(context, errorType, criticalityLevel, description);
            Issues.Add(newIssue);
        }

        private void CheckForNullSubItems(GraphSubAsset asset)
        {
            List<ISubAssetList> assetFields = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset).ToList();
            foreach (ISubAssetList assetList in assetFields)
            {
                for (int i = 0; i < assetList.SubAssets.Count; i++)
                {
                    GraphSubAsset subAsset = (GraphSubAsset)assetList.SubAssets[i];
                    if (subAsset == null)
                    {
                        AddAnalysis(asset, AnalysisType.NullSubAsset, $"SubAsset '{asset.name.AddRichTextColor(Color.yellow)}' has null element at position {i.ToStringBold().AddRichTextColor(Color.red)}.");
                    }
                    else
                    {
                        CheckForNullSubItems(subAsset);
                    }
                }
            }
        }

        public void FixErrors()
        {
            bool fixMissingBinding = false;

            if (Issues.Any(i => i.Type == AnalysisType.MissingBinding))
            {
                string title = "Unbind missing bindings variables?";
                string message = "Some parameters have lost their bindings. Keeping these errors can be useful as they make it easier to identify where they are, and you can open the Graph to fix them manually.\n\nDo you want to keep the missing parameter errors?";
                string unbindButton = "Unbind";
                string keepButton = "Keep";
                string cancelButton = "Cancel";

                int result = EditorUtility.DisplayDialogComplex(title, message, unbindButton, keepButton, cancelButton);
                if (result == 2)
                    return;

                fixMissingBinding = result == 0;
            }

            string path = AssetDatabase.GetAssetPath(GraphData);
            int corruptedObjects = AssetDatabase.RemoveScriptableObjectsWithMissingScript(path);

            if (corruptedObjects > 0)
            {
                Debug.Log($"Destroyed '{corruptedObjects}' null assets");
            }

            // Remove empty slots
            int nullCount = CollectionUtils.ClearNullObject(GraphData.SubAssets);

            if (nullCount > 0)
            {
                Debug.Log($"Removed '{nullCount}' null assets");
            }

            // Add missing items
            List<GraphSubAsset> subAssetList = EditorUtils.GetAllSubAssets<GraphSubAsset>(GraphData).ToList();
            List<Node> nodeList = subAssetList.OfType<Node>().ToList();

            // Add non listed nodes
            foreach (Node node in nodeList)
            {
                if (GraphData.SubAssets.Contains(node))
                    continue;

                GraphData.SubAssets.Add(node);
                Debug.Log($"Added node '{node.name}");
            }

            // Removed null items
            foreach (GraphSubAsset subAsset in GraphData.SubAssets)
            {
                RepairSubItemsRecursive(subAsset);
            }

            // Add non listed sub assets
            foreach (GraphSubAsset subAsset in subAssetList)
            {
                if (subAsset is Node)
                    continue;

                GraphSubAsset foundReference = null;
                foreach (Node nodes in nodeList)
                {
                    foundReference = ContainsSubAssetInLists(nodes, subAsset, true);
                    if (foundReference)
                        break;
                }

                if (foundReference)
                {
                    if (GraphData.SubAssets.Contains(subAsset))
                        continue;

                    GraphData.SubAssets.Add(subAsset);
                    Debug.Log($"Found missing reference of '{subAsset.name}'\nInside of '{foundReference.name}'");
                }
                else
                {
                    GraphData.SubAssets.Remove(subAsset);
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    Debug.Log($"Cound not find asset '{subAsset.name}', it will be removed");
                }
            }

            if (GraphData.StartNode == null)
            {
                Node node = GraphData.SubAssets.First(a => a is Node node && node.StartableNode) as Node;
                GraphData.SetStartNode(node);
                Debug.Log($"Included Start Node {node}");
            }

            // TODO: Fix repetition

            if (fixMissingBinding)
            {
                Dictionary<string, Variable> variables = GraphData.GetVariables().ToDictionary(v => v.guid, v => v);

                foreach (GraphSubAsset asset in GraphData.SubAssets)
                {
                    if (!asset)
                        continue;

                    asset.GetAllFieldValuesTypeOf<IParameter>().ForEach(p =>
                    {
                        if (!p.IsBinding || p.IsSelfBind)
                            return;

                        // Check if the variable exist and the type matches
                        if (!variables.TryGetValue(p.Guid, out Variable variable) || !TypeResolver.CanConvert(p, variable))
                        {
                            p.Unbind();
                        }
                    });
                }
            }

            // Save changes
            AssetDatabase.SaveAssets();

            // Reevaluate
            Refresh();
        }

        private void RepairSubItemsRecursive(GraphSubAsset asset)
        {
            int nullCount = 0;
            List<ISubAssetList> assetFields = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset).ToList();

            foreach (ISubAssetList assetList in assetFields)
            {
                IList subAssets = assetList.SubAssets;
                for (int i = 0; i < subAssets.Count;)
                {
                    GraphSubAsset subAsset = subAssets[i] as GraphSubAsset;

                    if (subAsset == null)
                    {
                        nullCount++;
                        subAssets.RemoveAt(i);
                        continue;
                    }

                    RepairSubItemsRecursive(subAsset);
                    i++;
                }
            }

            if (nullCount > 0)
            {
                Debug.Log($"Removed '{nullCount}' null sub asset from '{asset.name}'");
            }
        }

        private GraphSubAsset ContainsSubAssetInLists(GraphSubAsset target, GraphSubAsset assetToCheck, bool reparing)
        {
            List<ISubAssetList> assetFields = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(target).ToList();
            foreach (ISubAssetList assetList in assetFields)
            {
                foreach (GraphSubAsset subAsset in assetList.SubAssets)
                {
                    if (subAsset == assetToCheck)
                        return target;

                    if (!subAsset && !reparing) // Note: During repair this should not be null
                        continue;

                    GraphSubAsset foundedAsset = ContainsSubAssetInLists(subAsset, assetToCheck, reparing);
                    if (foundedAsset)
                        return foundedAsset;
                }
            }
            return null;
        }

        public List<IssueDetail> GetIssues(GraphSubAsset asset)
        {
            return Issues.Where(e => e.Context == asset).ToList();
        }
    }
}
