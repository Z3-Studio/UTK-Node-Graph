using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Correct a body based on current slope")]
    public class CorrectSlope : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [Header("Inputs")]
        [SerializeField] private Parameter<Vector3> slopeCheckPoint;
        [SerializeField] private Parameter<LayerMask> groundLayer;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<float> maxSlopeAngle = 45f;
        [SerializeField] private Parameter<float> slopeCheckDistance = 0.5f;

        protected override void StartAction()
        {
            float slopeDownAngle = CheckSlope();
            float currentDegrees = Mathf.MoveTowardsAngle(data.Value.eulerAngles.z, slopeDownAngle, DeltaTime * speed.Value);
            data.Value.rotation = Quaternion.Euler(data.Value.eulerAngles.x, data.Value.eulerAngles.y, currentDegrees);
            
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
                    return data.Value.right.x > 0 ? slopeAngle : -slopeAngle;
                }
            }

            return 0f;
        }
    }
}