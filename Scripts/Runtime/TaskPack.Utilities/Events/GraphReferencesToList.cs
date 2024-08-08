using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Converts a graph reference to a list of graph owners")]
    public class GraphReferencesToList : ActionTask
    {
        [SerializeField] private Parameter<GraphReferences> graphReference;
        [SerializeField] private Parameter<List<GraphRunner>> outOwners;

        public override string Info => $"Converting References to {outOwners}";

        protected override void StartAction()
        {
            //outOwners.Value = Agent.Graphs.Select(r => r.graphOwner).ToList();
            EndAction();
        }
    }
}