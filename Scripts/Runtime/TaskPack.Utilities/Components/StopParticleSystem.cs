using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Stop a particle system.")]
    public class StopParticleSystem : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<ParticleSystem> particleSystem;
        public override string Info => $"Stop {particleSystem}";

        [SerializeField] private Parameter<bool> stopChildren;

        protected override void StartAction()
        {
            particleSystem.Value.Stop(stopChildren.Value);
            EndAction();
        }
    }
}