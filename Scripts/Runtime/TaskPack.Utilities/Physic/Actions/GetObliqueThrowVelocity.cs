using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Return a Vector2 with the velocity for the oblique throw of a projectile. yLimits controls the min/max range of the throw.")]
    public class GetObliqueThrowVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [Header("In")]
        [SerializeField] private Parameter<Vector3> targetDistance;
        [SerializeField] private Parameter<float> forwardSpeed;
        [SerializeField] private Parameter<Vector2> speedYLimits;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> returnedVelocity;

        protected override void StartAction()
        {
            returnedVelocity.Value = MathUtils.ObliqueThrowX(targetDistance.Value, rigidbody.Value.mass, forwardSpeed.Value, speedYLimits.Value);
            EndAction();
        }
    }
}