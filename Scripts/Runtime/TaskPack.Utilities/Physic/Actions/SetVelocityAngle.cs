using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity by angle")]
    public class SetVelocityAngle : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;

        [SerializeField] private Parameter<Axis3> axis = Axis3.Z;
        [SerializeField] private Parameter<float> velocity;
        [SerializeField] private Parameter<float> angle;
        public override string Info => $"Velocity Angle = {velocity}";
        protected override void StartAction()
        {
            // TODO: Review, use axis?
            Quaternion redAxisRotation = Quaternion.AngleAxis(angle.Value, rigidbody.Value.transform.right);

            float finalAngle = redAxisRotation.eulerAngles.x + rigidbody.Value.transform.eulerAngles.y;

            rigidbody.Value.linearVelocity = MathUtils.AngleToDirection(finalAngle, velocity.Value);
            EndAction();
        }
    }
}