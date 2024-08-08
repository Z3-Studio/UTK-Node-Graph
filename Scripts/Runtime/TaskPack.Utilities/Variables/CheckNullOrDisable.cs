using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Useful to check if the object went to the pool or was destroyed")]
    public class CheckNullOrDisable : ConditionTask
    {
        [SerializeField] private Parameter<GameObject> variable;

        public override string Info => $"{variable} is Null or Disabled";

        public override bool CheckCondition()
        {
            return variable.Value.ObjectNullCheck() || !variable.Value.activeSelf;
        }
    }
}