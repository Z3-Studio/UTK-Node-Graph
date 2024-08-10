using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public static class NodeGraphEditorUtils
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
                foreach (GraphSubAsset subItem in subAssetList)
                {
                    DestroyAsset(graph, subItem);
                }
            }
        }

        public static List<GraphSubAsset> CollectAllDependencies(List<GraphSubAsset> assets)
        {
            List<GraphSubAsset> list = new List<GraphSubAsset>();

            foreach (GraphSubAsset asset in assets)
            {
                list.Add(asset);
                AddDependenciesRecursivily(list, asset);
            }

            return list;
        }

        public static void AddDependenciesRecursivily(List<GraphSubAsset> list, GraphSubAsset asset)
        {
            List<GraphSubAsset> dependencies = ReflectionUtils.GetAllFieldValuesTypeOf<ISubAssetList>(asset)
                .SelectMany(s => s.OfType<GraphSubAsset>())
                .ToList();

            list.AddRange(dependencies);

            foreach (GraphSubAsset dependency in dependencies)
            {
                AddDependenciesRecursivily(list, dependency);
            }
        }

        public static string ReplaceGuids(string name, Dictionary<string, string> dictionary)
        {
            int iterations = name.Count(c => c == '/') + 1;
            int startIndex = 0;

            for (int i = 0; i < iterations; i++)
            {
                int startGuid = name.IndexOf('[', startIndex);

                if (startGuid == -1)
                    break;

                int endGuid = name.IndexOf(']', startGuid);
                string oldGuid = name.Substring(startGuid + 1, endGuid - startGuid - 1);
                string newGuid = dictionary.GetValueOrDefault(oldGuid, oldGuid);

                name = name.Substring(0, startGuid + 1) + newGuid + name.Substring(endGuid);
                startIndex = endGuid + 1;
            }

            return name;
        }


        public static string ReplaceGuid(string name, string newGuid)
        {
            // Find the index of the last '[' and ']'
            int lastBracketStart = name.LastIndexOf('[');
            int lastBracketEnd = name.LastIndexOf(']');

            // Throw an exception if the brackets are not found or are in the wrong order
            if (lastBracketStart <= -1 || lastBracketStart >= lastBracketEnd)
            {
                throw new ArgumentException("The input string does not contain a valid GUID format enclosed by brackets.");
            }

            // Replace the old GUID with the new GUID
            return name.Substring(0, lastBracketStart + 1) + newGuid + name.Substring(lastBracketEnd);
        }

        public static void ReplaceParameterBinding(GraphSubAsset subAsset, Dictionary<string, string> guidVariables)
        {
            foreach (FieldInfo field in ReflectionUtils.GetAllFieldsTypeOf<IParameter>(subAsset))
            {
                IParameter parameter = field.GetValue(subAsset) as IParameter;

                if (guidVariables.TryGetValue(parameter.Guid, out string newVariableGuid))
                {
                    parameter.SetBinding(newVariableGuid);
                }
            }
        }

        // Replace values of: Parameters, List<SubAsset>, SubAsset, ISubAssetList (taskList, transitions)
        /*
        public static void Replace(GraphSubAsset subAsset, Dictionary<string, string> guidVariables, Dictionary<string, GraphSubAsset> newSubAssets)
        {
            ReplaceParameterBinding(subAsset, guidVariables);

            return;
            foreach (FieldInfo field in ReflectionUtils.GetAllFields(subAsset))
            {
                Type fieldType = field.FieldType;

                if (typeof(IParameter).IsAssignableFrom(fieldType))
                {
                    IParameter parameter = field.GetValue(subAsset) as IParameter;

                    if (guidVariables.TryGetValue(parameter.Guid, out string newGuid))
                    {
                        parameter.ReplaceDependencies(newGuid);
                    }
                }

                else if (typeof(GraphSubAsset).IsAssignableFrom(fieldType))
                {
                    GraphSubAsset fieldSubAsset = field.GetValue(subAsset) as GraphSubAsset;
                    if (!fieldSubAsset)
                        continue;

                    GraphSubAsset asset = newSubAssets.GetValueOrDefault(fieldSubAsset.Guid);
                    if (!asset)
                    {
                        Debug.LogError("NULL " + fieldSubAsset.name + " context " + subAsset.name, subAsset);
                        continue;
                    }

                    field.SetValue(subAsset, asset);
                }
                else if (typeof(ISubAssetList).IsAssignableFrom(fieldType))
                {
                    ISubAssetList list = field.GetValue(subAsset) as ISubAssetList;

                    IList oldList = list;

                    Type listType = oldList.GetType();

                    IList newList = Activator.CreateInstance(listType) as IList;


                    foreach (GraphSubAsset child in oldList)
                    {
                        GraphSubAsset asset = newSubAssets.GetValueOrDefault(child.Guid);
                        if (!asset)
                        {
                            Debug.LogError("NULL " + listType.Name + " subAsset " + subAsset.name, subAsset);
                            continue;
                        }

                        newList.Add(asset);
                    }

                    list.ReplaceAll(newList);

                    // Alternative
                    //list.ReplaceDependencies(newSubAssets);
                }
            }
        }*/

        /*
        // using NUnit.Framework;
        [Test]
        public void TestGuidReplacement_ValidInputs()
        {
            // Definir entradas
            string inputA = "SubGraphDataSM [c93fe2a49903e8c4b8120cda50b28492]/Transition [9bc565fc110f80d44a0d1c5c35662ec1]/CharacterControllerIsGrounded [0cbf3add8f88bda40958026bf0296c5b]";
            string inputB = "SubGraphDataSM [c93fe2a49903e8c4b8120cda50b28492]/Transition [9bc565fc110f80d44a0d1c5c35662ec1]/CheckBool [1f1a79d96d749644180555d1685b3152]";
            string inputC = "SubGraphDataSM [812e162e10f15f448b83970ab7e84c3b]/Transition [bb4e8679e00187449b4cba2ba716448a]";

            // Criar o dicionário com GUIDs antigos e novos GUIDs gerados
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "c93fe2a49903e8c4b8120cda50b28492", "AAAA0000AAAA0000AAAA0000AAAA0000" },
                { "9bc565fc110f80d44a0d1c5c35662ec1", "BBBB1111BBBB1111BBBB1111BBBB1111" },
                { "0cbf3add8f88bda40958026bf0296c5b", "CCCC2222CCCC2222CCCC2222CCCC2222" },
                { "1f1a79d96d749644180555d1685b3152", "DDDD3333DDDD3333DDDD3333DDDD3333" },
                { "812e162e10f15f448b83970ab7e84c3b", "EEEE4444EEEE4444EEEE4444EEEE4444" },
                { "bb4e8679e00187449b4cba2ba716448a", "FFFF5555FFFF5555FFFF5555FFFF5555" }
            };

            // Processar e gerar os outputs
            string outputA = GetGuid(inputA, dictionary);
            string outputB = GetGuid(inputB, dictionary);
            string outputC = GetGuid(inputC, dictionary);

            // Exibir os outputs
            Assert.AreEqual(outputA, "SubGraphDataSM [AAAA0000AAAA0000AAAA0000AAAA0000]/Transition [BBBB1111BBBB1111BBBB1111BBBB1111]/CharacterControllerIsGrounded [CCCC2222CCCC2222CCCC2222CCCC2222]");
            Assert.AreEqual(outputB, "SubGraphDataSM [AAAA0000AAAA0000AAAA0000AAAA0000]/Transition [BBBB1111BBBB1111BBBB1111BBBB1111]/CheckBool [DDDD3333DDDD3333DDDD3333DDDD3333]");
            Assert.AreEqual(outputC, "SubGraphDataSM [EEEE4444EEEE4444EEEE4444EEEE4444]/Transition [FFFF5555FFFF5555FFFF5555FFFF5555]");
        }
        */
    }
}