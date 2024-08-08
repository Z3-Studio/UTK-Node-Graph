using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class SetFloatTime : ActionTask
    {
        [SerializeField] private Parameter<float> value;

        public override string Info => $"{value} = Time.time";

        protected override void StartAction()
        {
            value.Value = Time.time;
            EndAction();
        }
    }
}