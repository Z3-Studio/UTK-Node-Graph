using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("GameObject.SetActive(active)")]
    public class SetActiveGameObject : ActionTask<Component> 
    {
        public Parameter<bool> active;

        public override string Info => $"{AgentInfo}.GameObject.Active = {active}";

        protected override void StartAction() {
            Agent.gameObject.SetActive(active.Value);
            EndAction(true);
        }
    }
}