using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Play a particle system.")]
    public class PlayParticleSystem : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<ParticleSystem> particleSystem;
        public override string Info => $"Play {particleSystem}";

        [SerializeField] private Parameter<bool> playChildren;

        protected override void StartAction()
        {
            particleSystem.Value.Play(playChildren.Value);
            EndAction();
        }
    }
}