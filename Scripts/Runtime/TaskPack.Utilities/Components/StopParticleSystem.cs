using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Stop a particle system.")]
    public class StopParticleSystem : ActionTask<ParticleSystem>
    {
        public override string Info => $"Stop {AgentInfo}";

        public Parameter<bool> stopChildren;

        protected override void StartAction()
        {
            Agent.Stop(stopChildren.Value);
            EndAction(true);
        }
    }
}