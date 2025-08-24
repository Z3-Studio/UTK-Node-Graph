using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Set element from list based in index")]
    public class SetListElementFromIndex : ActionTask
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private Parameter<int> index;
        [SerializeField] private Parameter<object> newValue;

        public override string Info => $"{list}[{index}] = {newValue}";

        protected override void StartAction()
        {
            list.Value[index.Value] = newValue.Value;
            EndAction();
        }
    }
}
