using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Gets the sign of a number (-1 if negative and 1 if positive)")]
    public class GetSign : ActionTask
    {
        public Parameter<float> inNumber;
        public Parameter<float> outSign;

        public override string Info => $"{outSign} = Sign of {inNumber}";

        protected override void StartAction()
        {
            outSign.Value = Mathf.Sign(inNumber.Value);
            EndAction(true);
        }
    }
}