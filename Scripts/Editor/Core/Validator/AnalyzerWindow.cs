using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.UIBuilder.ExtensionMethods;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    // Ideas
    // Fix Varibles by promoting and generating new guid
    // Sub assets show child erros count recursively
    // Click in error and show in Graph
    // Analyze by AnalysisCriticality and give logic insights, such unused nodes and variables
    // Create in user preferences, option of not automatically validate (giving performance)
    // Auto fix missing reference script by using sub asset name
    // Fix TaskList by auto fill using sub asset Name
    // Validate Sub Asset Name by checking Type. Warning
    // Fix Error display popup with Check box list by categories. Ex: Null assets, Missing parameters, etc...
    // Issue item has button to fix
    // Create automated test to build a graph with error 
    public class AnalyzerWindow : Z3EditorWindow, IHasCustomMenu
    {
        [UIElement] private Label graphDataLabel;

        [UIElement] private VisualElement variableDepedencyContainer;
        [UIElement] private VisualElement subAssetTreeContainer;
        [UIElement] private VisualElement graphInspectorContainer;
        [UIElement] private VisualElement subAssetInspectorContainer;
        [UIElement] private VisualElement issuesContainer;
        [UIElement] private VisualElement overviewLeftContainer;

        // Overview
        [UIElement] private Label assetsCount;
        [UIElement] private Label startNode;
        [UIElement] private Label referenceVariablesCount;
        [UIElement] private Label localVariablesCount;

        [UIElement] private Label corruptedAssets;
        [UIElement] private Label nullAssets;
        [UIElement] private Label notIncludedAssets;
        [UIElement] private Label invalidAssetGuids;
        [UIElement] private Label unreferencedAssets;
        [UIElement] private Label missingBindings;
        [UIElement] private Label otherIssues;
        [UIElement] private Label issuesCount;

        [UIElement] private Label issueDetails;

        private InspectorElement graphDataInspector;
        private InspectorElement subAssetInspector;

        // Analyzer
        private GraphData GraphData => analyzer.GraphData;

        private GraphDataAnalyzer analyzer;

        // Lock
        private GUIStyle lockButtonStyle;
        private bool locked;

        private const string WindowName = "Analyzer";
        private const int TreeItemHeight = 92;

        [MenuItem(GraphPath.MenuPath + WindowName)]
        public static void OpenWindow()
        {
            GetWindow<AnalyzerWindow>(WindowName);
        }

        public static void DisplayAsset(GraphData graphData)
        {
            GraphDataAnalyzer analyzer = Validator.GetAnalyzer(graphData);
            AnalyzerWindow instance = GetWindow<AnalyzerWindow>(WindowName);

            instance.analyzer = analyzer;
            instance.Populate();
        }

        protected override void CreateGUI()
        {
            NodeGraphResources.AnalizerWindowVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            // Prepare tabs
            graphDataInspector = new InspectorElement();
            graphInspectorContainer.Add(graphDataInspector);

            subAssetInspector = new InspectorElement();
            subAssetInspectorContainer.Add(subAssetInspector);

            // Populate automaticly
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (locked)
                return;

            GraphData selectedGraphData = null;

            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out GraphRunner runner))
            {
                selectedGraphData = runner.GraphData;
            }
            else if (Selection.activeObject is GraphData)
            {
                selectedGraphData = Selection.activeObject as GraphData;
            }

            if (selectedGraphData && (!analyzer || GraphData != selectedGraphData))
            {
                analyzer = Validator.GetAnalyzer(selectedGraphData);
            }

            Populate();
        }

        [UIElement("open-graph")]
        private void OnOpenGraph()
        {
            if (!analyzer)
                return;

            NodeGraphWindow.OpenGraph(analyzer.GraphData);
        }

        [UIElement("fix-errors-button")]
        private void OnFixErrors()
        {
            if (!analyzer)
                return;

            analyzer.FixErrors();
            Populate();
        }

        [UIElement("ping-object-button")]
        private void OnRevealInProject()
        {
            if (!analyzer)
                return;

            EditorGUIUtility.PingObject(GraphData);
        }

        [UIElement("refresh-button")]
        private void Populate()
        {
            // Clean
            subAssetTreeContainer.Clear();
            subAssetInspector.Unbind();
            subAssetInspector.Clear();

            issuesContainer.Clear();
            variableDepedencyContainer.Clear();

            OnSelectIssue(null);

            if (!analyzer)
            {
                overviewLeftContainer.style.SetDisplay(false);
                graphDataLabel.text = "Select a Graph".AddRichTextColor(Color.grey);
                return;
            }

            analyzer.Refresh();
            graphDataLabel.text = GraphData.name;
            overviewLeftContainer.style.SetDisplay(true);

            // Overview
            graphDataInspector.Bind(GraphData);

            // General overview
            assetsCount.text = GraphData.SubAssets.Count.ToString();
            startNode.text = GraphData.StartNode.ToString();
            startNode.tooltip = GraphData.StartNode.name.ToString();
            referenceVariablesCount.text = GraphData.ReferenceVariables ? GraphData.ReferenceVariables.GetAllVariables().Count.ToString() : "Null".ToItalic();
            localVariablesCount.text = GraphData.LocalVariables.Count.ToString();

            // Issues overview
            IEnumerable<IGrouping<AnalysisType, IssueDetail>> issuesGroup = analyzer.Issues.GroupBy(i => i.Type);

            corruptedAssets.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.CorruptedAsset));
            nullAssets.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.NullAsset));
            notIncludedAssets.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.NotIncludedAsset));
            invalidAssetGuids.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.InvalidAssetGuid));
            unreferencedAssets.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.UnreferencedAsset));
            missingBindings.text = NoneOrError(analyzer.Issues.Count(i => i.Type == AnalysisType.MissingBinding));

            HashSet<AnalysisType> others = new HashSet<AnalysisType>
            {
                AnalysisType.Other,
                AnalysisType.StartNodeNotDefined,
                AnalysisType.DuplicateAsset,
                AnalysisType.InvalidAssetName
            };
            otherIssues.text = NoneOrError(analyzer.Issues.Count(i => others.Contains(i.Type)));

            issuesCount.text = NoneOrError(analyzer.IssuesCount);

            // Sub Assets
            IEnumerable<Node> nodes = GraphData.SubAssets.OfType<Node>();

            TreeContainer<GraphSubAsset> tree = new TreeContainer<GraphSubAsset>(nodes, analyzer.GetDirectyDependencies);
            TreeViewConfig<GraphSubAsset, GraphSubAssetAnalyzerView> treeConfig = new TreeViewConfig<GraphSubAsset, GraphSubAssetAnalyzerView>()
            {
                FixedItemHeight = TreeItemHeight
            };

            TreeViewContainer<GraphSubAsset, GraphSubAssetAnalyzerView> subGraphTree = new TreeViewContainer<GraphSubAsset, GraphSubAssetAnalyzerView>(tree, treeConfig);
            subGraphTree.OnChangeSelection += OnChangeSelection;
            subGraphTree.OnMakeItem += (newItem) => newItem.Init(analyzer);

            subAssetTreeContainer.Add(subGraphTree);

            // Parameters
            List<VariableAnalyzer> depedencies = analyzer.VariableDependencies.Values.ToList();
            List<Column> variableColumns = new()
            {
                new Column()
                {
                    name = "variable-name",
                    title = "Variable Name", 
                    width = 300,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = depedencies[i].VariableName                    
                },  
                new Column()
                {
                    name = "guid",
                    title = "Guid",
                    width = 300,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = depedencies[i].VariableGuid
                    
                },
                new Column()
                {
                    name = "variable-type",
                    title = "Variable Type",
                    width = 150,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = depedencies[i].Variable?.OriginalType.Name

                },
                new Column()
                {
                    name = "variable-scope",
                    title = "Variable Scope",
                    width = 150,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = depedencies[i].VariableScope.ToNiceString()

                },
                new Column()
                {
                    name = "depedencies-in-graph",
                    title = "Depedencies in Graph",
                    width = 150,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = depedencies[i].Depedencies.Count.ToString()
                    
                }, 
                new Column()
                {
                    name = "depedencies-in-project",
                    title = "Depedencies in Project",
                    width = 150,
                    makeCell = () => new Label(),
                    bindCell = (e, i) => (e as Label).text = "?"
                },
            };

            Columns variableColumnsList = new Columns();
            variableColumns.ForEach(c => variableColumnsList.Add(c));

            MultiColumnListView variableTable = new MultiColumnListView(variableColumnsList); // Core
            variableTable.itemsSource = depedencies;

            variableDepedencyContainer.Add(variableTable);

            // Issues
            Z3ListViewConfig issuesListConfig = Z3ListViewConfig.SimpleTemplate<IssueItemView>();
            issuesListConfig.fixedItemHeight = 66;

            ListViewBuilder<IssueDetail, IssueItemView> issuesList = new ListViewBuilder<IssueDetail, IssueItemView>(analyzer.Issues, issuesListConfig);
            issuesContainer.Add(issuesList);
            issuesList.OnSelectChange += OnSelectIssue;

            OnSelectIssue(issuesList.Selection);
        }

        private string NoneOrError(int count) => count == 0 ? "None".ToItalic() : count.ToString().AddRichTextColor(Color.red);

        private void OnSelectIssue(IssueDetail issue)
        {
            issueDetails.text = issue == null ? string.Empty : issue.ToString();
        }

        private void OnChangeSelection(TreeItem<GraphSubAsset> assetSelected)
        {
            subAssetInspector.Unbind();
            subAssetInspector.Bind(assetSelected.Content);
        }

        #region Lock Button
        private void OnGUI()
        {
            lockButtonStyle = "IN LockButton";
        }

        /// <summary> Menu Context </summary>
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lock"), locked, () =>
            {
                locked = !locked;

                if (!locked)
                {
                    OnSelectionChange();
                }
            });
        }

        /// <summary> Lock Icon </summary>
        private void ShowButton(Rect position)
        {
            bool wasLocked = locked;
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);

            if (wasLocked && !locked)
            {
                OnSelectionChange();
            }
        }
        #endregion
    }
}