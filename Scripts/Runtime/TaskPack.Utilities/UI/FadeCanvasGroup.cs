using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Ui)]
    [NodeDescription("Drive the CanvasGroup alpha with an AnimationCurve")]
    public class FadeCanvasGroup : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<CanvasGroup> canvasGroup;
        [SerializeField] private Parameter<float> duration;

        [Tooltip("The curve value is the alpha, authored over normalized time (X from 0 to 1)")]
        [SerializeField] private Parameter<AnimationCurve> curve;

        private float elapsed;

        public override string Info => $"Fade {canvasGroup} ({duration}s)";

        protected override void StartAction()
        {
            elapsed = 0f;
        }

        protected override void UpdateAction()
        {
            elapsed += DeltaTime;

            if (elapsed >= duration.Value)
            {
                canvasGroup.Value.alpha = curve.Value.Evaluate(1f);
                EndAction();
                return;
            }

            canvasGroup.Value.alpha = curve.Value.Evaluate(elapsed / duration.Value);
        }
    }
}
