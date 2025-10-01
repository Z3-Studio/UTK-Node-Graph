using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    public class IsActiveGameObject : ConditionTask
    {
        [SerializeField] private Parameter<GameObject> gameObject;

        public override string InfoC => $"{gameObject} is Active";

        public override bool CheckCondition()
        {
            return gameObject.Value.activeInHierarchy;
        }
    }
}
