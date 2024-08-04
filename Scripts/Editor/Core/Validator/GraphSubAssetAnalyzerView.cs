using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.Editor
{
    public class GraphSubAssetAnalyzerView : VisualElement, IBindElement<TreeItem<GraphSubAsset>>
    {
        [UIElement] private Label title;
        [UIElement] private Label type;
        [UIElement] private Label guid;
        [UIElement] private Label bindedParameters;
        [UIElement] private Label issues;

        private GraphDataAnalyzer analyzer;

        public GraphSubAssetAnalyzerView()
        {
            NodeGraphResources.AnalizerSubAssetInfoVT.CloneTree(this);
            this.BindUIElements();
        }

        public void Init(GraphDataAnalyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public void Bind(TreeItem<GraphSubAsset> item, int index)
        {
            GraphSubAsset asset = item.Content;

            title.text = asset.ToString();
            type.text = asset.GetType().Name;

            List<IParameter> parameterList = asset.GetAllFieldValuesTypeOf<IParameter>().ToList();

            if (parameterList.Count == 0)
            {
                bindedParameters.text = "None";
            }
            else
            {
                bindedParameters.text = $"{parameterList.Count} / {parameterList.Count(p => p.IsBinding)}";
            }

            // Guid
            string assetName = asset.name;
            int lastSlashIndex = assetName.LastIndexOf('/');
            guid.text = asset.Guid;
            guid.tooltip = assetName.Substring(lastSlashIndex + 1);

            List<IssueDetail> issuesList = analyzer.GetIssues(asset);
            // TODO: Add child errors acount to parent recursively

            if (issuesList.Count == 0)
            {
                issues.text = "None";
            }
            else
            {
                issues.text = issuesList.Count.ToString().AddRichTextColor(Color.red);

                string issuesString = string.Empty;
                foreach (IssueDetail issue in issuesList)
                {
                    issuesString += issue.ToString() + "\n";
                }

                issuesString = issuesString[..^1];

                issues.tooltip = issuesString;
            }
        }

        //[UIElement]
        private void ShowInGraph()
        {
            // TODO: Open Node Graph and focus in object or node
        }
    }
}