using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Check an animator state")]
    public class CheckAnimation : ConditionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<int> layer;
        [SerializeField] private Parameter<EAnimationCheck> check = EAnimationCheck.Playing;
        [Tooltip("Count only after the state is seen playing")]
        [SerializeField] private Parameter<bool> requireEnter;

        private readonly AnimationStateWatch watch = new();

        public override string InfoC => $"{stateName} {check}{PlayAnimation.LayerLabel(layer)}";

        public override void StartCondition()
        {
            watch.Begin(animator.Value, stateName.Value, layer.Value, check.Value, requireEnter.Value);
        }

        public override bool CheckCondition()
        {
            return watch.IsMet();
        }
    }
}
