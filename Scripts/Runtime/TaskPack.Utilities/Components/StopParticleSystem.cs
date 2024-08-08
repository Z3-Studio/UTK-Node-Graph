using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Stop a particle system.")]
    public class StopParticleSystem : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<ParticleSystem> data;
        public override string Info => $"Stop {data}";

        [SerializeField] private Parameter<bool> stopChildren;

        protected override void StartAction()
        {
            data.Value.Stop(stopChildren.Value);
            EndAction();
        }
    }
}