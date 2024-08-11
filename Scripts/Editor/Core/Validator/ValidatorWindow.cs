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
        [UIElement] private ToolbarButton warningButton;
        [UIElement] private ToolbarButton errorButton;
        [UIElement] private VisualElement content;
        [UIElement] private Label errorDetails;

        private const string WindowName = "Validator";

        private readonly List<ValidatorLogView> validatorLog = new();

        [MenuItem(GraphPath.MenuPath + WindowName)]
        public static void OpenWindow()
        {
            GetWindow<ValidatorWindow>(WindowName);
        }

        private void CreateGUI()
        {
            NodeGraphResources.ValidatorWindowVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            PopulateErrors();

            // TODO: Scroll and highlight to object when selection
        }

        [UIElement("refresh-all-button")]
        private void OnValidateAll()
        {
            Validator.ValidateAll();
            PopulateErrors();
        }

        [UIElement("fix-all-button")]
        private void OnFixAll()
        {
            foreach (ValidatorLogView log in validatorLog)
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
                ValidatorLogView newLog = new ValidatorLogView(analyzer, OnSelectIssue);
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