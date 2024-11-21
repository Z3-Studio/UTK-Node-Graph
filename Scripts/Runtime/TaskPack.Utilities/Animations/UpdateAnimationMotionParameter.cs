using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name")]
    public class UpdateAnimationMotionParameter : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;

        [Header("Variables")]
        [SerializeField] private Parameter<Vector3> currentVelocity;
        [SerializeField] private Parameter<float> maxVelocityScale;
        [SerializeField] private Parameter<float> animationBlendDamp = 10f;

        [Header("Parameters")]
        [SerializeField] private Parameter<string> horizontalMagnitude = "HorizontalVelocity";
        [SerializeField] private Parameter<string> velocityX = "VelocityX";
        [SerializeField] private Parameter<string> velocityY = "VelocityY";
        [SerializeField] private Parameter<string> velocityZ = "VelocityZ";

        protected override void StartAction()
        {
            float maxScale = maxVelocityScale.Value == 0 ? 1 : maxVelocityScale.Value; // Avoid division by 0
            Vector3 velocityScale = currentVelocity.Value / maxScale;

            Vector2 speedXY = new Vector2(velocityScale.x, velocityScale.z);

            SetFloat(horizontalMagnitude.Value, speedXY.magnitude);
            SetFloat(velocityX.Value, velocityScale.x);
            SetFloat(velocityY.Value, velocityScale.y);
            SetFloat(velocityZ.Value, velocityScale.z);

            EndAction();
        }

        private void SetFloat(string parameter, float value)
        {
            float currentValue = animator.Value.GetFloat(parameter);
            float finalValue = Mathf.Lerp(currentValue, value, DeltaTime * animationBlendDamp.Value);

            if (finalValue <= 0.01f)
            {
                finalValue = 0;
            }

            animator.Value.SetFloat(parameter, finalValue);
        }
    }
}