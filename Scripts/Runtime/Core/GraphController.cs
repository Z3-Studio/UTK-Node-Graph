﻿using System.Collections.Generic;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Base class that contains the logic of how to execute a GraphData
    /// </summary>
    public abstract class GraphController
    {
        /// <summary> Clone, used only for editor classes </summary>
        public GraphData GraphData { get; }

        public IGraphRunner Runner { get; }
        public VariableInstanceList LocalVariables { get; }
        public VariableInstanceList ReferenceVariables => Runner.ReferenceVariables;

        protected internal GraphController(IGraphRunner runner, GraphData originalData)
        {
            Runner = runner;

            // Clone Local Variables
            LocalVariables = VariableInstanceList.CloneVariables(originalData.LocalVariables);

            // Clone Graph
            GraphData graph = originalData.CloneT();
            graph.name = originalData.name;

            graph.SubAssets.Clear();

            // Clone all sub assets
            Dictionary<string, GraphSubAsset> subAssetDic = new();
            foreach (GraphSubAsset originalAsset in originalData.SubAssets)
            {
                GraphSubAsset assetClone = originalAsset.CloneT();

                subAssetDic[originalAsset.Guid] = assetClone;
                graph.SubAssets.Add(assetClone);
            }

            // Bind asset dependencies
            foreach (GraphSubAsset selfInstance in graph.SubAssets)
            {
                selfInstance.SetupDependencies(this, subAssetDic);
            }

            graph.SetStartNode(subAssetDic[graph.StartNode.Guid] as Node);

            GraphData = graph;
        }

        public GraphController BuildSubGraph(GraphData subGraph) => subGraph.CreateInstance(Runner);

        public virtual void StartGraph() { }
        public abstract State OnUpdate();
        public virtual void StopGraph() { }

        public override string ToString() => $"{Runner.Component.name} - {GraphData.name}";
    }

    /// <summary>
    /// Used to facilitate the inheritance
    /// </summary>
    public abstract class GraphController<TGraphData> : GraphController where TGraphData : GraphData
    {
        public new TGraphData GraphData => (TGraphData)base.GraphData;

        public GraphController(IGraphRunner runner, TGraphData originalData) : base(runner, originalData) { }
    }
}
