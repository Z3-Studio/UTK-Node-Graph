using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class NodeVariablesPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NodeVariablesPanel, UxmlTraits> { }

        private NodeGraphReferences references;

        internal void Init(NodeGraphReferences nodeGraphReferences)
        {
            references = nodeGraphReferences;
            references.OnChangeGraph += OnChangeGraph;

            NodeGraphWindow.ForceRedrawGraph += ForceRedraw;
        }

        internal void ForceRedraw(GraphData graphData)
        {
            if (graphData != references.Data)
                return;

            OnChangeGraph(references.Data);
        }

        private void OnChangeGraph(GraphData graphData)
        {
            Clear();

            if (!graphData)
                return;

            if (references.GraphController != null)
            {
                DrawAsInstance(references.GraphController);
            }
            else
            {
                DrawAsOriginal(graphData);
            }
        }

        private void DrawAsOriginal(GraphData graphData)
        {
            // Create Reference Variables Object Field
            SerializedObject serializedObject = new SerializedObject(graphData);
            SerializedProperty property = serializedObject.FindProperty(GraphData.ReferenceVariablesField);
            PropertyField propertyField = new PropertyField(property);
            propertyField.Bind(serializedObject);

            // Reference Variable 
            VisualElement referenceVariablesSlot = new();
            CreateReferenceVariables(graphData, referenceVariablesSlot);

            propertyField.RegisterValueChangeCallback(e =>
            {
                CreateReferenceVariables(graphData, referenceVariablesSlot);
            });

            // Local Variables
            string tooltip = "Visible only in this graph, consider it as a variable declared inside a method";
            VariableList localVariables = new VariableList("Local Variables".ToBold(), graphData, graphData.LocalVariables, true, tooltip);
            localVariables.OnDelete += OnDeleteVariable;

            // Add Visual Elements
            Add(propertyField);
            TitleBuilder.AddTitle(this, string.Empty);
            Add(referenceVariablesSlot);
            TitleBuilder.AddTitle(this, string.Empty);
            Add(localVariables);
        }

        private void DrawAsInstance(GraphController graphController)
        {
            VariableInstanceListView referenceVariableList = new VariableInstanceListView(graphController.ReferenceVariables);
            VariableInstanceListView localvariableList = new VariableInstanceListView(graphController.LocalVariables);

            TitleBuilder.AddTitle(this, string.Empty);
            Add(referenceVariableList);
            TitleBuilder.AddTitle(this, string.Empty);
            Add(localvariableList);
        }

        private void CreateReferenceVariables(GraphData graphData, VisualElement root)
        {
            root.Clear();

            if (graphData.ReferenceVariables != null)
            {
                string tooltip = "Global variables can be used in multiple graphs, but their main function is to collect the name and GUID to enable binding in Parameter<T>";
                VariableList referenceVariables = new VariableList("Reference Variables".ToBold(), graphData.ReferenceVariables, graphData.ReferenceVariables.GetOriginalVariables(), true, tooltip);
                referenceVariables.OnDelete += OnDeleteVariable;
                root.Add(referenceVariables);
            }
            else
            {
                root.Add(new Label("No Reference Variables"));
            }
        }

        private void OnDeleteVariable()
        {
            bool valid = Validator.Validate(references.Data);

            // TODO: Update Nodes and Inspector
            if (!valid)
            {
                references.Refresh();
            }
        }
    }
}