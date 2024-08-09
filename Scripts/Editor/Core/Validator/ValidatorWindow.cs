using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    // TODO: Analyze Components and GraphVariables
    /// <summary> Used to find problems in GraphData </summary>
    /// <remarks> It not validate runtime changes </remarks>
    public class ValidatorWindow : EditorWindow
    {
        [UIElement] private ToolbarButton validateAllButton;
        [UIElement] private ToolbarButton fixAllButton;
        [UIElement] private ToolbarButton warningButton;
        [UIElement] private ToolbarButton errorButton;
        [UIElement] private VisualElement content;
        [UIElement] private Label errorDetails;

        private const string WindowName = "Validator";

        private readonly List<ValidatorLog> validatorLog = new();

        [MenuItem(GraphPath.MenuPath + WindowName)]
        public static void OpenWindow()
        {
            GetWindow<ValidatorWindow>(WindowName);
        }

        private void CreateGUI()
        {
            NodeGraphResources.ValidatorWindowVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            validateAllButton.clicked += OnValidateAll;
            fixAllButton.clicked += OnFixAll;

            PopulateErrors();

            // TODO: Scroll and highlight to object when selection
        }

        private void OnValidateAll()
        {
            Validator.ValidateAll();
            PopulateErrors();
        }

        private void OnFixAll()
        {
            foreach (ValidatorLog log in validatorLog)
            {
                log.OnFixAll();
            }
        }

        private void PopulateErrors()
        {
            content.Clear();
            validatorLog.Clear();

            foreach (GraphDataAnalyzer analyzer in Validator.GraphDataAnalyzers.Select(p => p.Value).Where(d => d.HasErrors))
            {
                ValidatorLog newLog = new ValidatorLog(analyzer, OnSelectIssue);
                validatorLog.Add(newLog);
                content.Add(newLog);
            }
        }

        private void OnSelectIssue(IssueDetail issue)
        {
            errorDetails.text = issue.ToString();
        }
    }
}