using System;
using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Get element from list based in index")]
    public class GetListElementFromIndex: ActionTask
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private Parameter<int> index;
        [SerializeField] private Parameter<object> result;

        public override string Info => $"Get {list}[{index}]";

        protected override void StartAction()
        {
            result.Value = list.Value[index.Value];
            EndAction();
        }
    }
}
