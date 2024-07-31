using System.Collections.Generic;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    [CreateAssetMenu(menuName = GraphPath.Graph + "Graph Variables", fileName = "New" + nameof(GraphVariables))]
    public sealed class GraphVariables : VariablesAsset
    {
        [Hide/*, ListDrawer(searchable: true)*/] // TODO: Remove HideInInspector in 2023.3
        [SerializeField] private List<Variable> variables;

        public List<Variable> Variables => variables;

        public override GraphVariables GetOriginal() => this;

        public override List<Variable> GetVariables() => variables;
    }
}
