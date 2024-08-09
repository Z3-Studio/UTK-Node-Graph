﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils;

namespace Z3.NodeGraph.Editor
{
    public class NodeGraphAssetProcessor : AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (assetPath.EndsWith(".asset"))
            {
                GraphData graphData = AssetDatabase.LoadAssetAtPath<GraphData>(assetPath);

                if (graphData)
                {
                    Validator.Remove(graphData);
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }

        private static void OnWillCreateAsset(string assetPath)
        {
            if (!assetPath.EndsWith(".asset"))
                return;

            EditorApplication.delayCall += DelayCall;

            void DelayCall()
            {
                EditorApplication.delayCall -= DelayCall;

                GameObject prefabRoot = AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;

                GraphData graphData = AssetDatabase.LoadAssetAtPath<GraphData>(assetPath);
                string fullPath = Path.GetFullPath(assetPath);

                if (!graphData)
                {
                    Debug.LogError($"Fail loading asset at path {assetPath}");
                    return;
                }

                UpdateNewAsset(graphData);

                Validator.Add(graphData);
            }

        }

        /// <summary> Used to repair clones, changing guids and depedencies </summary>
        private static void UpdateNewAsset(GraphData newGraphData)
        {
            // 1. Create new guid for LocalVariables
            // Key: Old Guid. Value: New Guid
            Dictionary<string, string> guidVariables = new();

            foreach (Variable originalLocalVariable in newGraphData.LocalVariables)
            {
                // Save old and new guid
                string originalGuid = originalLocalVariable.Guid;
                string newGuid = GUID.Generate().ToString();

                guidVariables[originalGuid] = newGuid;

                originalLocalVariable.guid = newGuid;
            }

            // 2. Generate new guids for SubAssets
            // Key: Old Guid. Value: New Guid
            Dictionary<string, string> guidAssets = new();

            foreach (GraphSubAsset originalAsset in newGraphData.SubAssets)
            {
                guidAssets[originalAsset.Guid] = GUID.Generate().ToString();
            }

            // 3. Replace guids
            // Key: Old Guid. Value: New Asset
            foreach (GraphSubAsset subAsset in newGraphData.SubAssets)
            {
                // Setup guid and name
                string newAssetGuid = guidAssets[subAsset.Guid];
                string newName = NodeGraphEditorUtils.ReplaceGuids(subAsset.name, guidAssets);

                int parentLastIndex = newName.LastIndexOf('/');
                string newParentName = newName.Substring(0, parentLastIndex + 1);

                subAsset.SetGuid(newAssetGuid, newParentName);

                // Setup parameters depedencies
                foreach (FieldInfo field in ReflectionUtils.GetAllFieldsTypeOf<IParameter>(subAsset))
                {
                    IParameter parameter = field.GetValue(subAsset) as IParameter;

                    if (guidVariables.TryGetValue(parameter.Guid, out string newVariableGuid))
                    {
                        parameter.ReplaceDependencies(newVariableGuid);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}