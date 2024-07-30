using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;


namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Returns Slope Direction Vector, Angle and boolean. Input Layer and the max angle that's considered a slope")]
    public class GetSlopeDirection : ActionTask<Transform>
    {
        [Header("Inputs")]
        [Tooltip("Slope Check Point")]
        public Parameter<Transform> slopeCheckPoint;
        [Tooltip("Collision layer to check for")]
        public Parameter<LayerMask> groundLayer;
        [Tooltip("Max angle to be considered a slope")]
        public Parameter<float> maxSlopeAngle;

        [Header("OutPuts")]
        //[BlackboardOnly]
        public Parameter<Vector2> slopeDirection;
        //[BlackboardOnly]
        public Parameter<float> slopeAngle;
        //[BlackboardOnly]
        public Parameter<bool> isCurrentlyOnSlope;


        public override string Info => "Get Slope Direction";

        protected override void StartAction()
        {
            float slopeDownAngle = 0f;
            bool isOnSlope = false;
            Vector2 slopeNormalPerpendicular = Vector2.zero;
            //Vector2 checkPosition = Agent.position - new Vector3(0f, collider.Value.size.y / 2 + collider.Value.offset.y);
            Vector2 checkPosition = slopeCheckPoint.Value.position;

            float slopeCheckDistance = 0.5f;

            RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, slopeCheckDistance, groundLayer.Value);

            if (hit)
            {
                slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
                slopeDownAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);

                if (Mathf.Abs(slopeDownAngle) >= maxSlopeAngle.Value)
                    isOnSlope = true;
                else
                    isOnSlope = false;

                Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.red);
                Debug.DrawRay(hit.point, hit.normal, Color.green);
            }

            slopeDirection.Value = slopeNormalPerpendicular;
            slopeAngle.Value = slopeDownAngle;
            isCurrentlyOnSlope.Value = isOnSlope;

            EndAction();
        }
    }
}