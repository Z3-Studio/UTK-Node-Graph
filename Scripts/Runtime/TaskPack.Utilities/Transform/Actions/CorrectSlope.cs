using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Correct a body based on current slope")]
    public class CorrectSlope : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [Header("Inputs")]
        [SerializeField] private Parameter<Vector3> slopeCheckPoint;
        [SerializeField] private Parameter<LayerMask> groundLayer;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<float> maxSlopeAngle = 45f;
        [SerializeField] private Parameter<float> slopeCheckDistance = 0.5f;

        protected override void StartAction()
        {
            float slopeDownAngle = CheckSlope();
            float currentDegrees = Mathf.MoveTowardsAngle(transform.Value.eulerAngles.z, slopeDownAngle, DeltaTime * speed.Value);
            transform.Value.rotation = Quaternion.Euler(transform.Value.eulerAngles.x, transform.Value.eulerAngles.y, currentDegrees);
            
            EndAction();
        }


        private float CheckSlope()
        {
            RaycastHit2D hit = Physics2D.Raycast(slopeCheckPoint.Value, Vector2.down, slopeCheckDistance.Value, groundLayer.Value);
            
            if (hit)
            {
                float slopeAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);

                if (Mathf.Abs(slopeAngle) <= maxSlopeAngle.Value)
                {
                    return transform.Value.right.x > 0 ? slopeAngle : -slopeAngle;
                }
            }

            return 0f;
        }
    }
}