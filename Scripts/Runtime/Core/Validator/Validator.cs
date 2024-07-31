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
            if (!GraphDataAnalyzers.TryGetValue(graphData, out GraphDataAnalyzer analyzer))
            {
                analyzer = new GraphDataAnalyzer(graphData);
                GraphDataAnalyzers[graphData] = analyzer;
            }

            return !analyzer.HasErrors;
        }


        /// <returns> Return true if is valid </returns>
        public static bool Reevaluate(GraphData graphData)
        {
            GraphDataAnalyzers[graphData].Reevaluate();
            return !GraphDataAnalyzers[graphData].HasErrors;
        }
    }
}
