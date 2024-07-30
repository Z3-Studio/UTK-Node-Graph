using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    /// <summary> Used to find problems in GraphData </summary>
    /// <remarks> It not validate runtime changes </remarks>
    public class ValidatorWindow : EditorWindow
    {
        [UIElement] private ToolbarButton validateAllButton;
        [UIElement] private ToolbarButton warningButton;
        [UIElement] private ToolbarButton errorButton;
        [UIElement] private VisualElement content;
        [UIElement] private Label errorDetails;

        private const string WindowName = "Validator";

        private GraphData graph;

        [MenuItem(GraphPath.MenuPath + WindowName)]
        public static void OpenWindow()
        {
            GetWindow<ValidatorWindow>(WindowName);
        }

        private void CreateGUI()
        {
            NodeGraphResources.ValidatorVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            validateAllButton.clicked += OnValidateAll;

            PopulateErrors();

            // Populate automaticly
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {

            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out GraphRunner runner))
            {
                graph = runner.GraphData;
            }
            else if (Selection.activeObject is GraphData)
            {
                graph = Selection.activeObject as GraphData;
            }
        }

        private void OnValidateAll()
        {
            Validator.ValidateAll();
            PopulateErrors();
        }

        private void PopulateErrors()
        {
            content.Clear();

            foreach (GraphDataAnalyzer analyzer in Validator.GraphDataAnalyzers.Select(p => p.Value).Where(d => d.HasErrors))
            {
                ValidatorLog newLog = new ValidatorLog(analyzer);
                content.Add(newLog);
            }
        }
    }
}