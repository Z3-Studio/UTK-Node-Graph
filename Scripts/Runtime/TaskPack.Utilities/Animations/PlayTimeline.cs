using UnityEngine.Playables;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play timeline by director")]
    public class PlayTimeline : ActionTask
    {
        public Parameter<PlayableDirector> playableDirector;
        public Parameter<bool> waitUntilFinish = true;

        public override string Info => $"► Timeline {playableDirector}";

        protected override void StartAction()
        {
            if (waitUntilFinish.Value)
            {
                playableDirector.Value.stopped += OnTimelineStopped;
                playableDirector.Value.Play();
            }
            else
            {
                playableDirector.Value.Play();
                EndAction();
            }
        }

        protected override void StopAction()
        {
            playableDirector.Value.stopped -= OnTimelineStopped;
        }

        private void OnTimelineStopped(PlayableDirector playableDirector)
        {
            playableDirector.stopped -= OnTimelineStopped;
            EndAction();
        }
    }
}
