using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    [CustomEditor(typeof(GraphVariablesComponent))]
    public class GraphVariablesComponentEditor : Z3Editor<GraphVariablesComponent>
    {
        private ToolbarSearchField searchField;
        private OverrideVariableList variableList;

        private VariableInstanceListView variableInstanceList;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(base.CreateInspectorGUI());

            if (!Target.HasAsset())
                return root;

            if (!Target.AssetIsValid())
            {
                Label error = new Label("Asset is invalida".AddRichTextColor(Color.red));
                root.Add(error);
                return root;
            }

            searchField = new();
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
            searchField.style.width = new Length(100, LengthUnit.Percent);

            root.Add(searchField);

            if (Application.isPlaying)
            {
                //if (Target.ReferenceVaribles == null)
                //{
                //    Target.InitReferenceVariables();
                //}
                variableInstanceList = new VariableInstanceListView(Target.ReferenceVariables);
                root.Add(variableInstanceList);
            }
            else
            {
                variableList = new OverrideVariableList(Target, Target.GetOverrideVariables(), Target.GetBaseVariables());
                root.Add(variableList);
            }


            return root;
        }

        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            return;
            // Get the text typed
            string searchText = evt.newValue;

            // Update the exibition list to only show content matching the search
            foreach (OverrideVariableView variableElement in variableList.GetItems())
            {
                bool visible = variableElement.Variable.name.SearchMatch(searchText) || variableElement.Variable.OriginalType.Name.SearchMatch(searchText);
                variableElement.style.SetDisplay(visible);
            }
        }

        private void PrefabIdeas()
        {
            // Get the root GameObject of the prefab
            GameObject rootPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(Target.gameObject);

            // Check if the component is part of a prefab instance
            if (PrefabUtility.IsPartOfPrefabInstance(Target))
            {
                // Get the corresponding prefab asset path
                string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Target.gameObject);

                // Get the root GameObject of the prefab asset
                GameObject rootPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);

                // Create serialized objects for the prefab asset and the prefab instance
                SerializedObject prefabAssetSerializedObject = new SerializedObject(rootPrefabAsset);
                SerializedObject prefabInstanceSerializedObject = new SerializedObject(rootPrefab);

                // Compare the properties of the prefab asset and the prefab instance
                SerializedProperty assetProperty = prefabAssetSerializedObject.GetIterator();
                SerializedProperty instanceProperty = prefabInstanceSerializedObject.GetIterator();
                while (instanceProperty.Next(true) && assetProperty.Next(true))
                {
                    if (SerializedProperty.EqualContents(instanceProperty, assetProperty))
                    {
                        continue;
                    }

                    EditorGUILayout.LabelField(instanceProperty.displayName);

                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(instanceProperty, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        prefabInstanceSerializedObject.ApplyModifiedProperties();
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}