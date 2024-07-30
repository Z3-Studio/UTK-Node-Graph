using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("MonoBehaviour.enable = active")]
    public class SetActiveComponent : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Behaviour> agent;
        [SerializeField] private Parameter<bool> active;

        public override string Info => $"{agent}.enable = {active}";

        protected override void StartAction() 
        {
            agent.Value.enabled = active.Value;
            EndAction(true);
        }
    }
}