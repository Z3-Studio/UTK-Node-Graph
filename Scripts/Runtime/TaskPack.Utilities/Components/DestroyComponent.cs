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
        /*[RequiredField]*/
        public Parameter<Component> prefab;

        public override string Info => prefab.Value == null ?
            base.Info : $"Destroy {prefab}";

        protected override void StartAction()
        {
            Destroy(prefab.Value);
            EndAction(true);
        }
    }
}