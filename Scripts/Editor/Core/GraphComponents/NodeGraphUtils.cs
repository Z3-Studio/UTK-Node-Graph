using Z3.NodeGraph.Core;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using Z3.Utils;
using UnityEngine;

namespace Z3.NodeGraph.Editor
{
    public static class NodeGraphUtils
    {
        public static T GetTarget<T>(SerializedProperty property) where T : Object // TODO: Extension method
        {
            return (T)property.serializedObject.targetObject;
        }
        public static GraphData GetGraphData(GraphSubAsset subAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(subAsset);
            return AssetDatabase.LoadAssetAtPath<GraphData>(assetPath);
        }

        public static void DeleteAssets<T>(GraphData graphData, List<T> assets) where T : GraphSubAsset
        {
            // Remember
            UndoRecorder.AddUndo(graphData, "Delete NG assets"); // Transitions

            foreach (T asset in assets)
            {
                DestroyAsset(graphData, asset);
            }

            AssetDatabase.SaveAssets();
        }

        public static void DeleteAsset(GraphSubAsset asset)
        {
            DeleteAsset(null, asset);
        }
        public static void DeleteAsset(GraphData graph, GraphSubAsset subAsset)
        {
            // Remember
            UndoRecorder.AddUndo(graph, "Delete NG assets"); // Nodes

            DestroyAsset(graph, subAsset);

            AssetDatabase.SaveAssets();
        }


        private static void DestroyAsset(GraphData graph, GraphSubAsset asset)
        {
            // TaskList + Transitions
            List<ISubAssetList> subAssetFields = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset).ToList();
            DeleteSubItemsRecursive(graph, subAssetFields);

            // Remove
            graph.RemoveSubAsset(asset);
            AssetDatabase.RemoveObjectFromAsset(asset);
        }

        /// <summary>
        /// Used to destroy sub items. 
        /// Example: ActionListSM -> ActionTaskList, TransitionList -> ConditionTaskList
        /// </summary>
        private static void DeleteSubItemsRecursive(GraphData graph, List<ISubAssetList> subAssetFields)
        {
            foreach (ISubAssetList subAssetList in subAssetFields)
            {
                foreach (GraphSubAsset subItem in subAssetList.SubAssets)
                {
                    DestroyAsset(graph, subItem);
                }
            }
        }
    }
}