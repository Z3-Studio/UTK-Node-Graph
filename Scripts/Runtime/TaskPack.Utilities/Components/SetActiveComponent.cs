using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("MonoBehaviour.enable = active")]
    public class SetActiveComponent : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Behaviour> component;
        [SerializeField] private Parameter<bool> active;

        public override string Info => $"{component}.enable = {active}";

        protected override void StartAction() 
        {
            component.Value.enabled = active.Value;
            EndAction();
        }
    }
}