using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public interface IDataSource
    {
        ScriptableObject Data { get; }
    }

    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("You can add a component and get directly")]
    public class MapParametersSource : MapParametersBase
    {
        [SerializeField] protected Parameter<IDataSource> source;

        protected override ScriptableObject Data => source.Value.Data;

        public override string Info => $"Map Parameters from {source}";
    }
}
