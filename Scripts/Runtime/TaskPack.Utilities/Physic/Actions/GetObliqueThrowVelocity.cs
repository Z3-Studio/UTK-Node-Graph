using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Return a Vector2 with the velocity for the oblique throw of a projectile. yLimits controls the min/max range of the throw.")]
    public class GetObliqueThrowVelocity : ActionTask<Rigidbody>
    {
        [Header("In")]
        public Parameter<Vector3> targetDistance;
        public Parameter<float> forwardSpeed;
        public Parameter<Vector2> speedYLimits;

        [Header("Out")]
        public Parameter<Vector3> returnedVelocity;

        protected override void StartAction()
        {
            returnedVelocity.Value = MathUtils.ObliqueThrowX(targetDistance.Value, Agent.mass, forwardSpeed.Value, speedYLimits.Value);
            EndAction(true);
        }
    }
}