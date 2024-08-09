using System.Collections.Generic;
using UnityEngine;
using Z3.Utils.Editor;

namespace Z3.NodeGraph.Core
{
    public static class Validator
    {
        public static Dictionary<GraphData, GraphDataAnalyzer> GraphDataAnalyzers { get; } = new();
        public static bool Initalized { get; private set; }

        public static void Init()
        {
            if (Application.isPlaying)
                return;

            Initalized = true;
            ValidateAll();
        }

        public static void ValidateAll()
        {
            GraphDataAnalyzers.Clear();

            List<GraphData> allGraphData = EditorUtils.GetAllAssets<GraphData>();
            foreach (GraphData graphData in allGraphData)
            {
                GraphDataAnalyzers[graphData] = new GraphDataAnalyzer(graphData);
            }
        }

        /// <returns> Return true if is valid </returns>
        public static bool Validate(GraphData graphData)
        {
            return GetErrorCount(graphData) == 0;
        }

        public static int GetErrorCount(GraphData graphData)
        {
            GraphDataAnalyzer analyzer = GetAnalyzer(graphData);
            return analyzer.Issues.Count;
        }

        /// <returns> Return true if is valid </returns>
        public static bool Refresh(GraphData graphData)
        {
            GraphDataAnalyzers[graphData].Refresh();
            return !GraphDataAnalyzers[graphData].HasErrors;
        }

        public static GraphDataAnalyzer GetAnalyzer(GraphData graphData)
        {
            // If is new asset, it may not have been analyzed yet
            // Note: For some reason, when you duplicate an item in the editor, the asset is deleted, resulting in a null GraphData in the analyzer
            if (!GraphDataAnalyzers.TryGetValue(graphData, out GraphDataAnalyzer analyzer) || !analyzer)
            {
                analyzer = new GraphDataAnalyzer(graphData);
                GraphDataAnalyzers[graphData] = analyzer;
            }

            return analyzer;
        }

        public static void Add(GraphData graphData)
        {
            GraphDataAnalyzer analyzer = new GraphDataAnalyzer(graphData);
            GraphDataAnalyzers[graphData] = analyzer;
        }

        public static void Remove(GraphData graphData)
        {
            GraphDataAnalyzer analyzer = GraphDataAnalyzers.GetValueOrDefault(graphData);
            analyzer?.Dispose();
            GraphDataAnalyzers.Remove(graphData);
        }
    }
}
