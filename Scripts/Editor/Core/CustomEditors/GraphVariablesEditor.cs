using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    [CustomEditor(typeof(GraphVariables))]
    public class GraphVariablesEditor : Z3Editor<GraphVariables>
    {
        private ToolbarSearchField searchField;

        private PropertyListField inheritedVariables;
        private OverrideVariableList overrideList;
        private VariableList declaredList;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(GetMonoScript());

            TitleView.AddTitle(root, "Graph Variables");

            SerializedProperty baseVariablesProperty = serializedObject.FindProperty("inheritedVariables");
            inheritedVariables = new PropertyListField(baseVariablesProperty);
            root.Add(inheritedVariables);

            searchField = new();
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
            searchField.style.marginTop = 8f;
            searchField.style.width = new Length(100, LengthUnit.Percent);
            root.Add(searchField);

            overrideList = new OverrideVariableList(Target, Target.OverrideVariables, Target.CloneBaseVariables());
            root.Add(overrideList);

            declaredList = new VariableList(Target, Target.DeclaredVariables);
            root.Add(declaredList);

            inheritedVariables.OnValueChanged += RedrawOverride;

            return root;
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(target);

            if (inheritedVariables != null)
            {
                inheritedVariables.OnValueChanged -= RedrawOverride;
            }
        }

        private void RedrawOverride()
        {
            Target.OnValidate();

            VisualElement root = overrideList.parent;
            int index = root.IndexOf(overrideList);
            root.Remove(overrideList);

            overrideList = new OverrideVariableList(Target, Target.OverrideVariables, Target.CloneBaseVariables());
            root.Insert(index, overrideList);

        }

        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            // Get the text typed
            string searchText = evt.newValue;

            // Update the exibition list to only show content matching the search
            foreach (VisualElement element in overrideList.GetItems())
            {
                OverrideVariableView variableElement = element.Q<OverrideVariableView>();
                bool visible = variableElement.Variable.name.SearchMatch(searchText) || variableElement.Variable.OriginalType.Name.SearchMatch(searchText);
                element.style.SetDisplay(visible);
            }

            foreach (VisualElement element in declaredList.GetItems())
            {
                VariableView variableElement = element.Q<VariableView>();
                bool visible = variableElement.Variable.name.SearchMatch(searchText) || variableElement.Variable.OriginalType.Name.SearchMatch(searchText);
                element.style.SetDisplay(visible);
            }
        }
    }
}