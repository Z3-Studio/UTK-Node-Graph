using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    /// <summary> Used to diagnose errors and anomalies </summary>
    public class GraphDataAnalyzer
    {
        public GraphData GraphData { get; }
        public Dictionary<GraphSubAsset, string> Errors { get; } = new();
        public bool HasErrors => Errors.Count > 0;

        public GraphDataAnalyzer(GraphData graphData)
        {
            GraphData = graphData;
            Validate();
        }

        internal void Reevaluate()
        {
            Errors.Clear();
            Validate();
        }

        private void Validate()
        {
            // Validate
            Dictionary<string, Variable> variables = GraphData.GetVariables().ToDictionary(v => v.guid, v => v);

            foreach (GraphSubAsset asset in GraphData.SubAssets)
            {
                if (asset == null) // TODO: Handle it
                    continue;

                asset.GetAllFieldValuesTypeOf<IParameter>().ForEach(p =>
                {
                    if (!p.IsBinding || p.IsSelfBind)
                        return;

                    // Check if the variable exist and the type matches
                    if (variables.TryGetValue(p.Guid, out Variable variable) && TypeResolver.CanConvert(p, variable))
                    {
                        p.Bind(variable);
                    }
                    else
                    {
                        p.Invalid();

                        string log = $"Parameter type of '{p.GenericType}' inside of the graph name of `{GraphData.name}' has an invalid binding.\nVariable GUID: {p.Guid.AddRichTextColor(Color.red)}\nAsset type '{asset.GetType().Name}' named as '{asset.name.AddRichTextColor(Color.yellow)}'";
                        AddError(asset, log);
                    }
                });
            }
        }

        private void AddError(GraphSubAsset asset, string log)
        {
            Errors[asset] = log;
            Debug.LogError(log, asset);
        }
    }
}
