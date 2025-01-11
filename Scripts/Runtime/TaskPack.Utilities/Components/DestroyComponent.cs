using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Components
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("This class allows to use Quaterion")]
    public class DestroyComponent : ActionTask
    {
        [Header("Spawn Pooled Object")]
        [SerializeField] private Parameter<Object> unityObject;

        public override string Info => $"Destroy {unityObject}";

        protected override void StartAction()
        {
            Destroy(unityObject.Value);
            EndAction();
        }
    }
}