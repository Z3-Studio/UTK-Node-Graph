using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name in all layers")]
    public class PlayAnimationAllLayers : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        //[Range(0, 1)]
        [SerializeField] private Parameter<float> transition = 0.25f;

        [SerializeField] private Parameter<bool> waitUntilFinish;
        //[ShowIf(nameof(waitUntilFinish), 1)]
        [SerializeField] private Parameter<int> waitLayer;

        public override string Info => $"► Play All: {stateName}";

        private AnimatorStateInfo stateInfo;
        private bool played;

        protected override void StartAction()
        {
            played = false;

            for (int i = 0; i <= animator.Value.layerCount; i++)
            {
                AnimatorStateInfo current = animator.Value.GetCurrentAnimatorStateInfo(i);
                animator.Value.CrossFade(stateName.Value, transition.Value / current.length, i);
            }

            if (!waitUntilFinish.Value)
            {
                EndAction();
            }
        }

        protected override void UpdateAction()
        {
            stateInfo = animator.Value.GetCurrentAnimatorStateInfo(waitLayer.Value);

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