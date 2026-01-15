using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Wait animator match state")]
    public class WaitAnimatorState : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<int> layerIndex;
        [SerializeField] private Parameter<bool> state = true;

        public override string Info => $"Wait Animator State {stateName} is {state}";

        protected override void StartAction()
        {
            Check();
        }

        protected override void UpdateAction()
        {
            Check();
        }

        private void Check()
        {
            if (animator.Value.IsState(stateName, layerIndex) == state.Value)
            {
                EndAction();
            }
        }
    }
}
