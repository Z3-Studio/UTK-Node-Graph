using UnityEngine.Video;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play a video clip")]
    public class PlayVideo : ActionTask
    {
        public Parameter<VideoPlayer> videoPlayer;
        public Parameter<VideoClip> videoClip;

        public override string Info => $"Play Video: {videoClip}";

        protected override void StartAction()
        {
            videoPlayer.Value.clip = videoClip.Value;
            videoPlayer.Value.loopPointReached += OnVideoFinish;
            videoPlayer.Value.Play();
        }

        protected override void StopAction()
        {
            videoPlayer.Value.loopPointReached -= OnVideoFinish;
        }

        private void OnVideoFinish(VideoPlayer videoPlayer)
        {
            videoPlayer.loopPointReached -= OnVideoFinish;
            videoPlayer.Stop();
            EndAction();
        }
    }
}
