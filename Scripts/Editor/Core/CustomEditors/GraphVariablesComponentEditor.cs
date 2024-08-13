using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.Editor;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    [CustomEditor(typeof(GraphVariablesComponent))]
    public class GraphVariablesComponentEditor : Z3Editor<GraphVariablesComponent>
    {
        private ToolbarSearchField searchField;
        private OverrideVariableList variableList;

        private VariableInstanceListView variableInstanceList;

        private VisualElement content;

        private const string OverrideVariablesField = "overrideVariables";
        private const string BaseVariablesAssetField = "baseVariablesAsset";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(GetMonoScript());

            SerializedProperty baseVariablesAsset = serializedObject.FindProperty(BaseVariablesAssetField);

            ObjectField objectField = new ObjectField(baseVariablesAsset.displayName)
            {
                value = Target.BaseVariablesAsset,
                objectType = typeof(GraphVariables),
            };

            objectField.BindProperty(baseVariablesAsset);
            objectField.RegisterValueChangedCallback(Redraw);

            root.Add(objectField);

            content = new VisualElement();
            root.Add(content);

            Draw();
            return root;
        }

        private void Draw()
        {
            if (!Target.BaseVariablesAsset)
                return;

            if (!Target.AssetIsValid())
            {
                Label error = new Label("Asset is invalid".AddRichTextColor(Color.red));
                content.Add(error);
                return;
            }

            searchField = new();
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
            searchField.style.width = new Length(100, LengthUnit.Percent);

            content.Add(searchField);

            if (Application.isPlaying)
            {
                Target.InitReferenceVariables(); // Force initilize avoid null reference

                variableInstanceList = new VariableInstanceListView(Target.ReferenceVariables);
                content.Add(variableInstanceList);
            }
            else
            {
                // TODO: Add SerializedProperties to Bind and display overrides
                variableList = new OverrideVariableList(Target, Target.GetOverrideVariables(), Target.GetBaseVariables());
                content.Add(variableList);
            }
        }

        private void Redraw(ChangeEvent<Object> e)
        {
            if (e.newValue == e.previousValue)
                return;

            content.Clear();
            Draw();
        }

        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            // Get the text typed
            string searchText = evt.newValue;

            // Update the exibition list to only show content matching the search
            foreach (OverrideVariableView variableElement in variableList.GetItems())
            {
                bool visible = variableElement.Variable.name.SearchMatch(searchText) || variableElement.Variable.OriginalType.Name.SearchMatch(searchText);
                variableElement.style.SetDisplay(visible);
            }
        }

        private void PrefabIdeas(SerializedProperty baseVariablesAsset)
        {
            SerializedProperty list = serializedObject.FindProperty(OverrideVariablesField);

            List<SerializedProperty> array = new List<SerializedProperty>();
            foreach (SerializedProperty item in list)
            {
                array.Add(list);
            }

            if (PrefabUtility.IsPartOfPrefabInstance(Target.gameObject))
            {
                GameObject prefabInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(Target.gameObject);

                List<PropertyModification> modifications = PrefabUtility.GetPropertyModifications(prefabInstance)
                    .Where(m => m.target is GraphVariablesComponent)
                    .ToList();


                List<ObjectOverride> overrideCounts = PrefabUtility.GetObjectOverrides(Target.gameObject);


                Regex ArrayPathRegex = new Regex(@"(.*?\[\d+\])", RegexOptions.Compiled);

                List<SerializedProperty> overrides = new List<SerializedProperty>();

                if (modifications.Any(m => m.propertyPath == baseVariablesAsset.propertyPath))
                {
                    overrides.Add(baseVariablesAsset);
                }


                foreach (PropertyModification modification in modifications)
                {
                    Match match = ArrayPathRegex.Match(modification.propertyPath);

                    if (match.Success)
                    {
                        SerializedProperty property = serializedObject.FindProperty(match.Value);
                        if (property != null)
                        {
                            overrides.Add(property);
                        }
                    }
                }

                if (overrides.Count > 0)
                {
                    PopupField<string> overrideDropdown = new PopupField<string>("Overrides", overrides.Select(p => p.propertyPath).ToList(), 0);
                    overrideDropdown.tooltip = "Temporary solution";
                    //root.Add(overrideDropdown);
                }
            }
        }
    }
}