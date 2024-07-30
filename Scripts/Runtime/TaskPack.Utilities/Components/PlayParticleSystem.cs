using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Play a particle system.")]
    public class PlayParticleSystem : ActionTask<ParticleSystem>
    {
        public override string Info => $"Play {AgentInfo}";

        public Parameter<bool> playChildren;

        protected override void StartAction()
        {
            Agent.Play(playChildren.Value);
            EndAction(true);
        }
    }
}