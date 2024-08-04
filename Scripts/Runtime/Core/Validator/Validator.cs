using System.Collections.Generic;
using Z3.Utils.Editor;

namespace Z3.NodeGraph.Core
{
    public static class Validator
    {
        public static Dictionary<GraphData, GraphDataAnalyzer> GraphDataAnalyzers { get; } = new();
        // TODO: Components and Variables

        public static bool Initalized { get; private set; }

        internal static void Init()
        {
            Initalized = true;
            ValidateAll();
        }

        public static void ValidateAll()
        {
            GraphDataAnalyzers.Clear();

            List<GraphData> allGraphData = EditorUtils.GetAllAssets<GraphData>();
            foreach (GraphData graphData in allGraphData)
            {
                GraphDataAnalyzer analyzer = new GraphDataAnalyzer(graphData);
                GraphDataAnalyzers[graphData] = analyzer;
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
            if (!GraphDataAnalyzers.TryGetValue(graphData, out GraphDataAnalyzer analyzer))
            {
                analyzer = new GraphDataAnalyzer(graphData);
                GraphDataAnalyzers[graphData] = analyzer;
            }

            return analyzer;
        }
    }
}
