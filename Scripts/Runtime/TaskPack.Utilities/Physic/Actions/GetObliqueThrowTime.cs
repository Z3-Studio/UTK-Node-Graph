using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Return a Vector2 with the velocity for the oblique throw of a projectile. yLimits controls the min/max range of the throw.")]
    public class GetObliqueThrowTime : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [Header("In")]
        [SerializeField] private Parameter<Vector3> targetDistance;
        [SerializeField] private Parameter<float> time;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> returnedVelocity;

        protected override void StartAction()
        {
            returnedVelocity.Value = MathUtils.ObliqueThrowTime(targetDistance.Value, rigidbody.Value.mass, time.Value);
            EndAction();
        }
    }
}