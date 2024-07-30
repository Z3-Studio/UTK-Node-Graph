using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name")]
    public class PlayAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<Animator> animator;

        public Parameter<string> stateName;
        [Slider(0, 1)]
        public Parameter<float> transition = 0.25f;
        public Parameter<bool> waitUntilFinish;
        public Parameter<int> layer;

        public override string Info => waitUntilFinish.Value ?
            $"► Playing {stateName}" :
            $"► Play {stateName}";

        private AnimatorStateInfo stateInfo;
        private bool played;

        protected override void StartAction()
        {
            played = false;

            AnimatorStateInfo current = animator.Value.GetCurrentAnimatorStateInfo(layer.Value);
            animator.Value.CrossFade(stateName.Value, transition.Value / current.length, layer.Value);

            if (!waitUntilFinish.Value)
            {
                EndAction(true);
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
                    EndAction(true);
                }
            }
            else if (played)
            {
                EndAction(true);
            }
        }
    }
}