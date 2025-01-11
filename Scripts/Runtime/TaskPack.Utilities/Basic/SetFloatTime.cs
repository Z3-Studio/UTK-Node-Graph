using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public class SetFloatTime : ActionTask
    {
        [SerializeField] private Parameter<float> value;
        [SerializeField] private Parameter<float> extra;

        public override string Info => $"{value} = Time.time {(extra.IsBinding ? extra : extra.Value == 0 ? string.Empty : $"+ ({extra})")}";

        protected override void StartAction()
        {
            value.Value = Time.time + extra.Value;
            EndAction();
        }
    }
}