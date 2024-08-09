using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class ValidatorLogView : VisualElement
    {
        [UIElement] private Label contextLabel;
        [UIElement] private Button revealButton;
        [UIElement] private Button fixAllButton;
        [UIElement] private VisualElement errorsContainer;
        [UIElement] private Foldout foldout;

        private GraphDataAnalyzer analyzer;
        private Action<IssueDetail> onSelectIssue;

        /// <summary> Link error color #3D89D4 </summary>
        private readonly Color selectErrorColor = new Color(0.172549f, 0.3647059f, 0.5294118f);

        public ValidatorLogView(GraphDataAnalyzer analyzer, Action<IssueDetail> onSelectIssue)
        {
            NodeGraphResources.ValidatorLogVT.CloneTree(this);
            this.BindUIElements();

            this.analyzer = analyzer;
            this.onSelectIssue = onSelectIssue;
            contextLabel.text = analyzer.GraphData.name;

            Populate();
        }

        [UIElement("open-in-analyzer-button")]
        private void OnOpenInAnalyzer()
        {
            AnalyzerWindow.DisplayAsset(analyzer.GraphData);
        }

        [UIElement("reveal-button")]
        private void OnReveal()
        {
            EditorGUIUtility.PingObject(analyzer.GraphData);
        }

        [UIElement("fix-all-button")]
        public void OnFixAll()
        {
            analyzer.FixErrors();
            errorsContainer.Clear();
            Populate();
        }

        private void Populate()
        {
            errorsContainer.Clear();
            foldout.text = $"Errors: {analyzer.Issues.Count}";

            Z3ListViewConfig listConfig = Z3ListViewConfig.SimpleTemplate<IssueItemView>();
            listConfig.fixedItemHeight = 52;

            ListViewBuilder<IssueDetail, IssueItemView> customListView = new ListViewBuilder<IssueDetail, IssueItemView>(analyzer.Issues, listConfig);
            customListView.OnSelectChange += onSelectIssue;
            errorsContainer.Add(customListView);
        }
    }
}