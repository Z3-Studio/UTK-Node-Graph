using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder;

namespace Z3.NodeGraph.Editor
{
    public class NodeGraphReferences
    {
        public static event Action<INodeGraphElement> OnInspect;

        public event Action<GraphData> OnChangeGraph;

        public EditorWindow Window { get; private set; }
        public NodeGraphPanel Graph { get; private set; }

        public VisualElement Inspector { get; private set; }
        public NodeVariablesPanel Variables { get; private set; }

        public GraphData Data { get; private set; }
        public GraphPreference Preferences { get; private set; }
        public GraphController GraphController { get; private set; } // nullable
        public NodeGraphModule Module { get; private set; }

        private BreadcrumbView BreadcrumbView { get; }

        public NodeGraphReferences(NodeGraphWindow window, NodeGraphPanel graph, VisualElement inspector, BreadcrumbView breadcrumbView, NodeVariablesPanel variables)
        {
            // Core
            Window = window;
            Graph = graph;
            BreadcrumbView = breadcrumbView;

            // Optional
            Inspector = inspector;
            Variables = variables;

            graph.Init(this);
            variables.Init(this);
        }

        public void Refresh() => SetGraphData(Data, GraphController);

        public void OpenGraphData(GraphData graphData, GraphController graphController)
        {
            BuildBreadcrumb(graphData);
            SetGraphData(graphData, graphController);
        }

        public void InvokeUpdateSelection(INodeGraphElement visualElement)
        {
            Module.Inspect(visualElement);
            OnInspect?.Invoke(visualElement);
        }

        public void ForceRedraw() => Graph.ForceRedraw();

        internal void DisposeModule()
        {
            Module?.Dispose();
        }

        private void SetGraphData(GraphData graphData, GraphController graphController)
        {
            GraphController = graphController;
            Data = graphData;

            DisposeModule();

            Module = ModuleCreator.GetModule(graphData, this);
            Module.OnInitialize();

            // Bind variables and validate
            if (graphData)
            {
                Validator.Validate(graphData);
                Preferences = UserPreferences.GetGraphPreference(graphData);
            }
            else
            {
                Preferences = null;
            }

            //BuildBreadcrumb(graphData);
            OnChangeGraph?.Invoke(graphData);
        }

        #region Breadcrumb
        private void BuildBreadcrumb(GraphData graph)
        {
            BreadcrumbView.ClearBreadcrumbs();

            if (!graph)
                return;

            BreadcrumbView.AddBreadcrumb(graph.name, () => SetGraphData(graph, GraphController));
        }

        internal void OpenSubGraph(ISubGraphNode subGraph)
        {
            if (Application.isPlaying) 
            {
                // Get instance of GraphData
                BreadcrumbView.AddAndSelectBreadcrumb(subGraph.SubController.GraphData.name, () => SetGraphData(subGraph.SubController.GraphData, subGraph.SubController));
            }
            else
            {
                // Get scriptable object reference of GraphData
                BreadcrumbView.AddAndSelectBreadcrumb(subGraph.SubGraph.name, () => SetGraphData(subGraph.SubGraph, null));
            }
        }
        #endregion
    }
}