using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class SetBoolean : ActionTask
    {
        [SerializeField] private Parameter<bool> value;
        [SerializeField] private BoolOperation operation = BoolOperation.Toggle;

        public override string Info => operation == BoolOperation.Toggle ? $"{value} = !{value}" : $"{value} = {operation}";

        protected override void StartAction()
        {
            value.Value = operation switch
            {
                BoolOperation.True => true,
                BoolOperation.False => false,
                BoolOperation.Toggle => !value.Value,
                _ => throw new NotImplementedException()
            };

            EndAction();
        }
    }
}