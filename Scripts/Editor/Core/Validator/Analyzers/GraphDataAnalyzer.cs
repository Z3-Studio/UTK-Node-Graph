using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils;
using Z3.Utils.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor // TODO: It should be editor class
{
    /// <summary> Used to diagnose errors and anomalies </summary>
    public class GraphDataAnalyzer : IDisposable
    {
        public GraphData GraphData { get; }
        public List<IssueDetail> Issues { get; } = new(); // Maybe ObservableCollection
        public bool HasErrors => Issues.Count > 0;
        public int IssuesCount => Issues.Count;
        public bool IsValid => GraphData;

        public Dictionary<string, VariableAnalyzer> VariableDependencies { get; } = new();

        /// <summary> Size of <see cref="GUID"/> </summary>
        private const int GuidSize = 32;

        public GraphDataAnalyzer(GraphData graphData)
        {
            GraphData = graphData;
            graphData.OnValidateRequested += Validate;

            Validate();

            if (Issues.Count == 0)
                return;

            string log = $"The graph name of `{GraphData.name}' has {Issues.Count.ToStringBold()} errors. Open Validator to see actions";
            Debug.LogError(log, GraphData);
        }

        public void Dispose()
        {
            Issues.Clear();
            VariableDependencies.Clear();

            GraphData.OnValidateRequested -= Validate;
        }

        public void Refresh()
        {
            Validate();
        }

        public List<GraphSubAsset> GetDirectyDependencies(GraphSubAsset asset)
        {
            return ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset)
                .SelectMany(s => s.OfType<GraphSubAsset>())
                .ToList();
        }

        private void Validate()
        {
            if (!GraphData)
            {
                // TODO: Remove from list
                return;
            }

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

                        string log = $"Parameter type '{p.GenericType}' has an invalid binding.\nVariable GUID: {parameterGuid.AddRichTextColor(Color.red)}\nAsset type: '{asset.GetType().Name}' named '{asset.name.AddRichTextColor(Color.yellow)}'";

                        // asset
                        AddAnalysis(asset, AnalysisType.MissingBinding, log, AnalysisCriticality.Warning);
                    }
                });
            }

            // Note: Is important to Bind variables in runtime to see the names in editor. But as GraphData can be cloned, it should not be validated
            if (Application.isPlaying)
            {
                // TODO: Find clean solution
                VariableDependencies.Clear();
                Issues.Clear();
                return;
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
                    AddAnalysis(null, AnalysisType.NullAsset, $"SubAsset list contains a null element at position {i.ToStringBold().AddRichTextColor(Color.red)}.");
                }
            }

            // 3.1 Assets not included
            IEnumerable<GraphSubAsset> allSubAssets = EditorUtils.GetAllSubAssets<GraphSubAsset>(GraphData);
            List<GraphSubAsset> missingSubAssets = allSubAssets.Except(GraphData.SubAssets).ToList();

            foreach (GraphSubAsset subAsset in missingSubAssets)
            {
                AddAnalysis(subAsset, AnalysisType.NotIncludedAsset, $"SubAsset {subAsset.name.AddRichTextColor(Color.yellow)} is not present in the SubAsset list");
            }

            // 3.2 Invalid GUID
            foreach (GraphSubAsset subAsset in GraphData.SubAssets)
            {
                if (!subAsset)
                    continue;

                string name = subAsset.name;
                string guid = subAsset.Guid;

                // Validate if the GUID matches the substring from the end of the name. It ignores the last ']'
                if (string.IsNullOrEmpty(guid) || name.Length + 2 < GuidSize || name.Substring(name.Length - GuidSize - 1, GuidSize) != guid)
                {
                    AddAnalysis(subAsset, AnalysisType.InvalidAssetGuid, $"SubAsset {subAsset.name.AddRichTextColor(Color.yellow)} has an invalid Guid: '{subAsset.Guid}'");
                }
            }

            // 4. Check missing nodes
            List<GraphSubAsset> subAssetList = EditorUtils.GetAllSubAssets<GraphSubAsset>(GraphData).ToList();
            List<Node> nodeList = subAssetList.OfType<Node>().ToList();


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
                    AddAnalysis(subAsset, AnalysisType.UnreferencedAsset, $"SubAsset '{subAsset.name.AddRichTextColor(Color.red)}' is not a Node and is not referenced by any other assets.");
                }
            }

            // 7. Check if StartNode is set
            // Note: Review StateMachineData.AddSubAsset
            if (GraphData.StartNode == null && GraphData.GetAnyStartableNode())
            {
                AddAnalysis(null, AnalysisType.StartNodeNotDefined, "The graph contains Nodes, but there is no definition for the StartNode.", AnalysisCriticality.Warning);
            }

            // 8. Repeted items
            foreach (GraphSubAsset asset in GraphData.SubAssets.GetDuplicates())
            {
                if (!asset)
                    continue;

                AddAnalysis(asset, AnalysisType.DuplicateAsset, $"SubAsset '{asset.name.AddRichTextColor(Color.red)}' appears more than once in the SubAsset list.", AnalysisCriticality.Error);
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
                for (int i = 0; i < assetList.Count; i++)
                {
                    GraphSubAsset subAsset = (GraphSubAsset)assetList[i];
                    if (subAsset == null)
                    {
                        AddAnalysis(asset, AnalysisType.NullAsset, $"SubAsset '{asset.name.AddRichTextColor(Color.yellow)}' contains a null element at position {i.ToStringBold().AddRichTextColor(Color.red)}.");
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

            // 1. Parameters
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

            // 2. Corrupted scripts
            string path = AssetDatabase.GetAssetPath(GraphData);
            int corruptedObjects = AssetDatabase.RemoveScriptableObjectsWithMissingScript(path);

            if (corruptedObjects > 0)
            {
                Debug.Log($"Destroyed '{corruptedObjects}' null assets");
            }

            // 3. Remove null slots
            int nullCount = CollectionUtils.ClearNullObject(GraphData.SubAssets);

            if (nullCount > 0)
            {
                Debug.Log($"Removed '{nullCount}' null assets");
            }

            // 3.2 Fix GUID
            foreach (GraphSubAsset subAsset in GraphData.SubAssets)
            {
                if (!subAsset)
                    continue;

                string name = subAsset.name;
                string guid = subAsset.Guid;

                // Validate if the GUID matches the substring from the end of the name. It ignores the last ']'
                if (string.IsNullOrEmpty(guid) || name.Length + 2 < GuidSize || name.Substring(name.Length - GuidSize - 1, GuidSize) != guid)
                {
                    int parentLastIndex = name.LastIndexOf('/');
                    string parentName = name.Substring(0, parentLastIndex + 1);

                    guid = name.Substring(name.Length - GuidSize - 1, GuidSize);

                    subAsset.SetGuid(guid, parentName);
                }
            }

            // 3.1 Add non listed assets
            IEnumerable<GraphSubAsset> allSubAssets = EditorUtils.GetAllSubAssets<GraphSubAsset>(GraphData);
            List<GraphSubAsset> missingSubAssets = allSubAssets.Except(GraphData.SubAssets).ToList();

            foreach (GraphSubAsset asset in missingSubAssets)
            {
                GraphData.SubAssets.Add(asset);
                Debug.Log($"Added node '{asset.name}");
            }

            // 5. Repair null items
            foreach (GraphSubAsset subAsset in GraphData.SubAssets)
            {
                // TODO: Improve it. Instructions inside of method
                RepairSubItemsRecursive(subAsset);
            }

            List<Node> nodeList = GraphData.SubAssets.OfType<Node>().ToList();

            // Add non listed sub assets
            foreach (GraphSubAsset subAsset in GraphData.SubAssets.ToList())
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
                Node node = GraphData.GetAnyStartableNode();
                if (node)
                {
                    GraphData.SetStartNode(node);
                    Debug.Log($"Included Start Node {node}");
                }
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

            foreach (ISubAssetList subAssets in assetFields)
            {
                for (int i = 0; i < subAssets.Count;)
                {
                    GraphSubAsset subAsset = subAssets[i] as GraphSubAsset;

                    // TODO: Before remove, try to find some asset that have good match. Example, use childAsset.name.Containes(parent.name) to fix null fields
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
                foreach (GraphSubAsset subAsset in assetList)
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

        public override string ToString() => GraphData ? GraphData.ToString() : "NULL";

        public static implicit operator bool(GraphDataAnalyzer analyzer) => analyzer != null && analyzer.IsValid;
    }
}
