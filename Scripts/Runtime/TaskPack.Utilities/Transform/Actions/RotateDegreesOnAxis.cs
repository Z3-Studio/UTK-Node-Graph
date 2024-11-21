using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate axis of a GameObject")]
    public class RotateDegreesOnAxis : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<float> degrees;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<Axis3> axis;

        public override string Info => $"Rotate {transform} {axis} {degrees} degrees";

        private float currentDegrees;
        private Vector3 initialAngle;

        private Func<Quaternion> updateRotation;
        protected override void StartAction()
        {
            currentDegrees = 0;
            initialAngle = transform.Value.eulerAngles;

            updateRotation = () => axis.Value switch
            {
                Axis3.X => Quaternion.Euler(currentDegrees + initialAngle.x, transform.Value.eulerAngles.y, transform.Value.eulerAngles.z),
                Axis3.Y => Quaternion.Euler(transform.Value.eulerAngles.x, currentDegrees + initialAngle.y, transform.Value.eulerAngles.z),
                Axis3.Z => Quaternion.Euler(transform.Value.eulerAngles.x, transform.Value.eulerAngles.y, currentDegrees + initialAngle.z),
                _ => throw new NotImplementedException(),
            };
        }

        protected override void UpdateAction()
        {
            currentDegrees = Mathf.MoveTowards(currentDegrees, degrees.Value, DeltaTime * speed.Value);
            transform.Value.rotation = updateRotation();

            if (currentDegrees == degrees.Value)
            {
                EndAction();
            }
        }
    }
}