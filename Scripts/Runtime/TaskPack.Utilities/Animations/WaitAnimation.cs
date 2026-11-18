using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Wait until an animator state meets the check")]
    public class WaitAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<int> layer;
        [SerializeField] private Parameter<EAnimationCheck> check = EAnimationCheck.ClipEnd;
        [Tooltip("Count only after the state is seen playing")]
        [SerializeField] private Parameter<bool> requireEnter = true;

        private readonly AnimationStateWatch watch = new();

        public override string Info => $"Wait {stateName} {check}{AnimationStateWatch.LayerLabel(layer)}";

        protected override void StartAction()
        {
            watch.Begin(animator.Value, stateName.Value, layer.Value, check.Value, requireEnter.Value);
        }

        protected override void UpdateAction() // Same tick as Start, so it can end there
        {
            if (watch.IsMet())
            {
                EndAction();
            }
        }
    }
}
