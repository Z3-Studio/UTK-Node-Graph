using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name in all layers")]
    public class PlayAnimationAllLayers : ActionTask<Animator>
    {
        public Parameter<string> stateName;
        //[Range(0, 1)]
        public Parameter<float> transition = 0.25f;

        public Parameter<bool> waitUntilFinish;
        //[ShowIf(nameof(waitUntilFinish), 1)]
        public Parameter<int> waitLayer;

        public override string Info => $"► Play All: {stateName}";

        private AnimatorStateInfo stateInfo;
        private bool played;

        protected override void StartAction()
        {
            played = false;

            for (int i = 0; i <= Agent.layerCount; i++)
            {
                AnimatorStateInfo current = Agent.GetCurrentAnimatorStateInfo(i);
                Agent.CrossFade(stateName.Value, transition.Value / current.length, i);
            }

            if (!waitUntilFinish.Value)
            {
                EndAction(true);
            }
        }

        protected override void UpdateAction()
        {
            stateInfo = Agent.GetCurrentAnimatorStateInfo(waitLayer.Value);

            if (stateInfo.IsName(stateName.Value))
            {

                played = true;
                if (NodeRunningTime >= (stateInfo.length / Agent.speed))
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