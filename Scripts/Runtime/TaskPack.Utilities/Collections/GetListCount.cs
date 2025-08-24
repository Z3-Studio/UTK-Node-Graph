using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Get list count")]
    public class GetListCount : ActionTask
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private Parameter<int> count;

        public override string Info => $"{count} = {list}.Count";

        protected override void StartAction()
        {
            count.Value =  list.Value.Count;
            EndAction();
        }
    }
}
