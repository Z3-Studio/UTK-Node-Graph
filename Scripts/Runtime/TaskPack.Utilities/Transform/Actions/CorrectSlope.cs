using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Correct a body based on current slope")]
    public class CorrectSlope : ActionTask<Transform>
    {
        [Header("Inputs")]
        public Parameter<Vector3> slopeCheckPoint;
        public Parameter<LayerMask> groundLayer;
        public Parameter<float> speed;
        public Parameter<float> maxSlopeAngle = 45f;
        public Parameter<float> slopeCheckDistance = 0.5f;

        protected override void StartAction()
        {
            float slopeDownAngle = CheckSlope();
            float currentDegrees = Mathf.MoveTowardsAngle(Agent.eulerAngles.z, slopeDownAngle, Time.fixedDeltaTime * speed.Value);
            Agent.rotation = Quaternion.Euler(Agent.eulerAngles.x, Agent.eulerAngles.y, currentDegrees);
            
            EndAction(true);
        }


        private float CheckSlope()
        {
            RaycastHit2D hit = Physics2D.Raycast(slopeCheckPoint.Value, Vector2.down, slopeCheckDistance.Value, groundLayer.Value);
            
            if (hit)
            {
                float slopeAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);

                if (Mathf.Abs(slopeAngle) <= maxSlopeAngle.Value)
                {
                    return Agent.right.x > 0 ? slopeAngle : -slopeAngle;
                }
            }

            return 0f;
        }
    }
}