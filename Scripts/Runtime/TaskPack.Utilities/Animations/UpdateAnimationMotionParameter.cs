using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by state name")]
    public class UpdateAnimationMotionParameter : ActionTask<Animator>
    {
        [Header("Variables")]
        public Parameter<Vector3> currentVelocity;
        public Parameter<float> maxVelocityScale;
        public Parameter<float> animationBlendDamp;

        [Header("Parameters")]
        public Parameter<string> velocityMagnitude = "MoveSpeed";
        public Parameter<string> velocityX = "VelocityX";
        public Parameter<string> velocityY = "VelocityY";
        public Parameter<string> velocityZ = "VelocityZ";

        protected override void StartAction()
        {
            float maxScale = maxVelocityScale.Value == 0 ? 1 : maxVelocityScale.Value; // Avoid division by 0
            Vector3 velocityScale = currentVelocity.Value / maxScale;

            SetFloat(velocityMagnitude.Value, velocityScale.magnitude);
            SetFloat(velocityX.Value, velocityScale.x);
            SetFloat(velocityY.Value, velocityScale.y);
            SetFloat(velocityZ.Value, velocityScale.z);

            EndAction();
        }

        private void SetFloat(string parameter, float value) => Agent.SetFloat(parameter, value, animationBlendDamp.Value, Time.fixedDeltaTime);
    }
}