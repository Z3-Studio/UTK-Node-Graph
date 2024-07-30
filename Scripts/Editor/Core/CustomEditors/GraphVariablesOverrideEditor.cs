using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    [CustomEditor(typeof(GraphVariablesOverride))]
    public class GraphVariablesOverrideEditor : Z3Editor<GraphVariablesOverride>
    {
        private ToolbarSearchField searchField;
        private OverrideVariableList variableList;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(base.CreateInspectorGUI());

            searchField = new();
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
            searchField.style.width = new Length(100, LengthUnit.Percent);

            variableList = new OverrideVariableList(Target, Target.GetOverrideVariables(), Target.GetBaseVariables());

            root.Add(searchField);
            root.Add(variableList);

            return root;
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
    }
}