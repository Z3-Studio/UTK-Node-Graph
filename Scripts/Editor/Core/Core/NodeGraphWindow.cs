using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using Z3.UIBuilder;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils.ExtensionMethods;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    public class NodeGraphWindow : EditorWindow, IHasCustomMenu
    {
        [UIElement] private NodeGraphPanel nodeGraphPanel;
        [UIElement] private VisualElement selectGraphContainer;
        [UIElement] private BreadcrumbView breadcrumbView;
        [UIElement(optional: true)] private VisualElement nodeInspectorPanel;
        [UIElement(optional: true)] private NodeVariablesPanel nodeVariablesPanel;

        public static event Action<GraphData> ForceRedrawGraph;

        private GraphData GraphData => nodeGraphReferences.Data;
        private NodeGraphReferences nodeGraphReferences;

        // Lock Button
        private GUIStyle lockButtonStyle;
        private bool locked;

        private const string NodeGraphs = "Node Graph";

        [MenuItem(GraphPath.MenuPath + "Graph")]
        public static void OpenWindow()
        {
            GetWindow<NodeGraphWindow>(NodeGraphs);
        }

        [OnOpenAsset]
        public static bool OpenEditor(int instanceId, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is GraphData)
            {
                OpenWindow();
                return false;
            }

            return false;
        }

        public static void OpenGraph(GraphData graphData)
        {
            NodeGraphWindow window = GetWindow<NodeGraphWindow>(NodeGraphs);
            window.nodeGraphReferences.OpenGraphData(graphData);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStageChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStageChanged;
            minSize = new Vector2(500, 200);
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStageChanged;

            nodeGraphReferences.DisposeModule();
            nodeGraphReferences = null;
        }

        private void OnPlayModeStageChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                rootVisualElement.Clear();
                CreateGUI();
            }
        }

        private void CreateGUI()
        {
            // Prevents bug where a second window content, is built together with the existing window, resulting in a visual bug that also deletes sub assets
            if (rootVisualElement.childCount > 0)
            {
                rootVisualElement.Clear();
                nodeGraphReferences?.DisposeModule();
            }

            NodeGraphResources.NodeGraphVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);
            selectGraphContainer.style.SetDisplay(true);

            nodeGraphReferences = new(this, nodeGraphPanel, nodeInspectorPanel, breadcrumbView, nodeVariablesPanel);

            nodeGraphPanel.styleSheets.Add(NodeGraphResources.NodeGraphSS); // Bug: Used to force the background grid get the style

            // Populate automaticly
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (locked)
                return;

            GraphData graph = null;
            GraphController controller = null;

            bool checkId = true;

            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out GraphRunner runner))
            {
                // TODO: Review it
                if (Application.isPlaying)
                {
                    graph = runner.RootController.GraphData;
                    controller = runner.RootController;
                    checkId = false;
                }
                else
                {
                    graph = runner.GraphData;
                }
            }
            else if (Selection.activeObject is GraphData)
            {
                graph = Selection.activeObject as GraphData;
            }
            
            if (graph && graph != GraphData)
            {
                selectGraphContainer.style.SetDisplay(false);

                // RootNodeBug? After create a soon as possible
                // TODO: Understand why and when this is necessary and document it
                if (checkId && !AssetDatabase.CanOpenAssetInEditor(graph.GetInstanceID())) 
                    return;

                nodeGraphReferences.OpenGraphData(graph, controller);
            }
        }

        [UIElement("open-in-analyzer-button")]
        private void OnOpenInAnalyzer()
        {
            if (nodeGraphReferences == null)
                return;

            AnalyzerWindow.DisplayAsset(GraphData);
        }

        [UIElement("refresh-button")]
        private void OnRefresh()
        {
            if (nodeGraphReferences == null)
                return;

            nodeGraphReferences.Refresh();
        }

        [UIElement("ping-object-button")]
        private void OnRevealInProject()
        {
            if (nodeGraphReferences == null)
                return;

            EditorGUIUtility.PingObject(GraphData);
        }

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
            });
        }

        /// <summary> Lock Icon </summary>
        private void ShowButton(Rect position)
        {
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }

        public static void ForceRedrawVariables(GraphData data)
        {
            ForceRedrawGraph?.Invoke(data);
        }
    }
}