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
        private VariableList variableList;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(base.CreateInspectorGUI());

            //PropertyField property = visualElement.Q<PropertyField>("variables");
            // bool ok = false;

            //property.RegisterCallback<GeometryChangedEvent>(_  =>
            //{
            //    if (ok)
            //        return;

            //    ListView listView = property.Q<ListView>();
            //    if (listView == null)
            //    {
            //        Debug.Log("fail");
            //        return;
            //    }

            //    listView.onAdd = (_) =>
            //    {
            //        List<(string, Type)> types = TypeResolver.CachedVariables;
            //        types.Add(("☆ Insert Title", typeof(Title)));
            //        SelectorPopup<Type>.OpenWindow("New Variable", types, OnAddNewVariable);
            //    };
            //    listView.allowRemove = false;

            //    ok = true;
            //});

            //return visualElement;

            searchField = new();
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
            searchField.style.width = new Length(100, LengthUnit.Percent);

            variableList = new(Target, Target.Variables);

            root.Add(searchField);
            root.Add(variableList);

            return root;
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(target);
        }

        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            // Get the text typed
            string searchText = evt.newValue;

            // Update the exibition list to only show content matching the search
            
            foreach (VisualElement element in variableList.GetItems())
            {
                VariableView variableElement = element.Q<VariableView>();
                bool visible = variableElement.Variable.name.SearchMatch(searchText) || variableElement.Variable.OriginalType.Name.SearchMatch(searchText);
                element.style.SetDisplay(visible);
            }
        }
    }
}