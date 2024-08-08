using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Best way to check if some object is null")]
    public class CheckNull : ConditionTask 
    {
        [SerializeField] private Parameter<object> variable;

        public override string Info  => $"{variable} == null";

        public override bool CheckCondition() 
        {
            // Unity fake null
            return variable.Value.ObjectNullCheck();
        }
    }
}