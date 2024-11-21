using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Movement
{

    [NodeCategory(Categories.Movement)]
    [NodeDescription("Move a GameObject to the target position, through an animationCurve.")]
    public class LerpPositionWithCurve : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> targetPosition;
        [SerializeField] private Parameter<float> time;
        [SerializeField] private Parameter<AnimationCurve> animationCurve;

        private Vector2 initPosition;
        private Vector2 finalPosition;
        private float t;

        public override string Info => $"Lerping To {targetPosition}";

        protected override void StartAction()
        {
            initPosition = transform.Value.position;
            finalPosition = targetPosition.Value;
            t = 0f;
        }

        protected override void UpdateAction()
        {
            t += DeltaTime / time.Value;
            transform.Value.position = Vector2.Lerp(initPosition, finalPosition, animationCurve.Value.Evaluate(t));

            if (t >= 1)
                EndAction();
        }
    }
}