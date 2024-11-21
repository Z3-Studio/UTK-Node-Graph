using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name")]
    public class PlayAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;

        [SerializeField] private Parameter<string> stateName;
        [Slider(0, 1)]
        [SerializeField] private Parameter<float> transition = 0.25f;
        [SerializeField] private Parameter<bool> waitUntilFinish;
        [SerializeField] private Parameter<int> layer;

        public override string Info => waitUntilFinish.Value ?
            $"► Playing {stateName}" :
            $"► Play {stateName}";

        private AnimatorStateInfo stateInfo;
        private bool played;

        protected override void StartAction()
        {
            played = false;

            animator.Value.PlayState(stateName.Value, transition.Value, layer.Value);

            if (!waitUntilFinish.Value)
            {
                EndAction();
            }
        }

        protected override void UpdateAction()
        {
            stateInfo = animator.Value.GetCurrentAnimatorStateInfo(layer.Value);

            if (stateInfo.IsName(stateName.Value))
            {
                played = true;
                if (NodeRunningTime >= (stateInfo.length / animator.Value.speed))
                {
                    EndAction();
                }
            }
            else if (played)
            {
                EndAction();
            }
        }
    }
}