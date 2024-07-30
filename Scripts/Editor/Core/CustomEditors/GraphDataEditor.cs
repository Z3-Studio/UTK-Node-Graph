using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils;

namespace Z3.NodeGraph.Editor
{
    // Note: Inside of BehaviourTreeDataEditor, There was a repair method for Composite, where it removed repeated items using Distinct().
    // Might be interested in a repair interface like IConnectedNode, containing a list and a boolean

    [CustomEditor(typeof(GraphData), true)]
    public class GraphDataEditor : NGEditor<GraphData>
    {
        public override void CreateInspector()
        {
            base.CreateInspector(); 
            VariableList variableList = new VariableList("Local Variables", Target, Target.LocalVariables);
            Add(variableList, true); 
            
            Label repairTools = TitleBuilder.GetTitle("Repair Tools");
            Add(repairTools, true);
            ReloadBtn();
        }

        private void ReloadBtn()
        {
            Button reload = new Button();
            reload.text = "Restore and Clear Sub Assets";
            reload.clicked += OnReload;

            Add(reload, true);
        }

        private void OnReload()
        {
            string path = AssetDatabase.GetAssetPath(target);
            int corruptedObjects = AssetDatabase.RemoveScriptableObjectsWithMissingScript(path);

            if (corruptedObjects > 0)
            {
                Debug.Log($"Destroyed '{corruptedObjects}' null assets");
            }

            // Remove empty slots
            int nullCount = CollectionUtils.ClearNullObject(Target.SubAssets);

            if (nullCount > 0)
            {
                Debug.Log($"Removed '{nullCount}' null assets");
            }

            // Add missing items
            List<GraphSubAsset> subAssetList = EditorUtils.GetAllSubAssets<GraphSubAsset>(Target).ToList();
            List<Node> nodeList = subAssetList.OfType<Node>().ToList();

            // Add non listed nodes
            foreach (Node node in nodeList)
            {
                if (Target.SubAssets.Contains(node))
                    continue;

                Target.SubAssets.Add(node);
                Debug.Log($"Added node '{node.name}");
            }

            // Removed null items
            foreach (GraphSubAsset subAsset in Target.SubAssets)
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
                    foundReference = ContainsSubAssetInLists(nodes, subAsset);
                    if (foundReference)
                        break;
                }

                if (foundReference)
                {
                    if (Target.SubAssets.Contains(subAsset))
                        continue;

                    Target.SubAssets.Add(subAsset);
                    Debug.Log($"Found missing reference of '{subAsset.name}'\nInside of '{foundReference.name}'");
                }
                else
                {
                    Target.SubAssets.Remove(subAsset);
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    Debug.Log($"Cound not find asset '{subAsset.name}', it will be removed");
                }
            }

            if (Target.StartNode == null)
            {
                Node node = Target.SubAssets.First(a => a is Node) as Node;
                Target.SetStartNode(node);
                Debug.Log($"Included Start Node {node}");
            }

            // Salva as alterações no asset
            AssetDatabase.SaveAssets();
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

        private GraphSubAsset ContainsSubAssetInLists(GraphSubAsset target, GraphSubAsset assetToCheck)
        {
            List<ISubAssetList> assetFields = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(target).ToList();

            foreach (ISubAssetList assetList in assetFields)
            {
                foreach (GraphSubAsset subAsset in assetList.SubAssets)
                {
                    // Founded
                    if (subAsset == assetToCheck)
                        return target;

                    // Check in children
                    GraphSubAsset foundedAsset = ContainsSubAssetInLists(subAsset, assetToCheck);
                    if (foundedAsset)
                        return foundedAsset;
                }
            }

            return null;
        }
    }
}