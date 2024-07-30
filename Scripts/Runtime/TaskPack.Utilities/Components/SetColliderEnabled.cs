using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    public class SetColliderEnabled : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Collider> collider;
        [SerializeField] private Parameter<bool> enabled;

        public override string Info => $"{collider}.enable = {enabled}";

        protected override void StartAction()
        {
            collider.Value.enabled = enabled.Value;
            EndAction();
        }
    }
}