using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Play a particle system.")]
    public class PlayParticleSystem : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<ParticleSystem> data;
        public override string Info => $"Play {data}";

        [SerializeField] private Parameter<bool> playChildren;

        protected override void StartAction()
        {
            data.Value.Play(playChildren.Value);
            EndAction();
        }
    }
}